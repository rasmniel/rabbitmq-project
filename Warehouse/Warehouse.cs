using System;
using System.Collections.Generic;
using System.Linq;
using EasyNetQ;
using Messages;
using Catalogue;

namespace Warehouse
{
    public class Warehouse
    {
        private int id;
        private string country;
        private ProductCatalogue catalogue;

        private const int BaseStock = 10;

        public Warehouse(int id, string country)
        {
            this.id = id;
            this.country = country;
            int stock = id == -1 ? 0 : BaseStock;
            catalogue = new ProductCatalogue(stock);
        }

        public void Listen()
        {
            using (IBus bus = RabbitHutch.CreateBus("host=localhost;persistentMessages=false"))
            {
                bus.Receive<OrderRequest>("warehouse_" + country, (message) =>
                {
                    Console.WriteLine("Received request from retailer");
                    handleOrderRequest(message);
                    OrderResponse replyMessage = getOrderResponse(message);
                    bus.Send(message.Sender, replyMessage);
                });

                bus.Subscribe<OrderRequest>("warehouseid_" + country, (message) =>
                {
                    OrderResponse replyMessage = getOrderResponse(message);
					bus.Send("order_response", replyMessage);
                });

                Console.WriteLine("Listening for order requests\n");
                Console.ReadLine();
            }
        }

        private OrderResponse getOrderResponse(OrderRequest message)
        {
            int daysForDelivery = country == message.Country ? 2 : 10;
            decimal shippingCharge = country == message.Country ? 5 : 10;

            Product requestedProduct = catalogue.getProductById(message.ProductId);

            return new OrderResponse()
            {
                WarehouseId = id,
                OrderId = message.OrderId,
                Stock = requestedProduct.Stock,
                DaysForDelivery = daysForDelivery,
                ShippingCharge = shippingCharge
            };
        }

        private void handleOrderRequest(OrderRequest message)
        {
            lock (this)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Order request received:");
                Console.WriteLine("Order Id: " + message.OrderId);
                Console.WriteLine("Product Id: " + message.ProductId);
                Console.WriteLine("Country: " + message.Country);
                Console.ResetColor();
            }
        }
    }
}
