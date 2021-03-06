﻿using System;
using EasyNetQ;
using Messages;
using Catalogue;
using EasyNetQ.Topology;

namespace Retailer
{
    public class Retailer
    {
        private static int _orderNumber = 1;
        private ProductCatalogue catalogue;

        public Retailer()
        {
            catalogue = new ProductCatalogue(1);
        }

        public void Listen()
        {
            using (IBus bus = RabbitHutch.CreateBus("host=localhost;persistentMessages=false"))
            {
                bus.Receive<ProductRequest>("customer_product_request", (message) =>
                {
                    Console.WriteLine(message.Product + " requested by " + message.Sender);
                    OrderRequest requestMessage = getOrderRequest(message);
                    bus.Send("warehouse_" + message.Country, requestMessage);

                    bus.Receive<OrderResponse>(requestMessage.Sender, (response) =>
                    {
                        if (response.Stock > 0)
                        {
                            ProductResponse responseMessage = getProductResponse(response);
                            Console.WriteLine("Sending product info to " + message.Sender);
                            bus.Send(message.Sender, responseMessage);
                        }
                        else
                        {
                            Console.WriteLine("No stock for " + message.Product);
                            bus.Publish(requestMessage);
                        }
                    });

                    bool foundStock = false;
                    bus.Receive<OrderResponse>("order_response", (response) =>
                    {
                        if (!foundStock && response.Stock > 0)
                        {
                            Console.WriteLine("Received product with stock");
                            foundStock = true;
                            ProductResponse product = getProductResponse(response);
                            bus.Send(message.Sender, product);
                        }
                    });
                });

                Console.WriteLine("Waiting for customer order.");
                Console.ReadLine();
            }
        }

        private OrderRequest getOrderRequest(ProductRequest message)
        {
            return new OrderRequest()
            {
                OrderId = _orderNumber++,
                ProductId = catalogue[message.Product].ID,
                Country = message.Country,
                Sender = message.Sender + _orderNumber
            };
        }

        private ProductResponse getProductResponse(OrderResponse response)
        {
            return new ProductResponse()
            {
                OrderId = response.OrderId,
                DaysForDelivery = response.DaysForDelivery,
                ShippingCharge = response.ShippingCharge
            };
        }

        private void handleOrderResponse(OrderResponse message)
        {
            lock (this)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Reply received from warehouse" + message.WarehouseId);
                Console.WriteLine("Order Id: " + message.OrderId);
                Console.WriteLine("Items in stock: " + message.Stock);
                Console.WriteLine("Days for delivery: " + message.DaysForDelivery);
                Console.WriteLine("Shipping charge: " + message.ShippingCharge);
                Console.ResetColor();
            }
        }

    }
}
