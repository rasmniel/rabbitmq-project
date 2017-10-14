using System;
using EasyNetQ;
using Messages;
using System.Threading;

namespace Retailer
{
    class RetailerProgram
    {
        static Object lockObject = new Object();
        static int orderNumber = 1;
        //private static int timeoutInterval = 30000;

        static void Main(string[] args)
        {
            using (var bus = RabbitHutch.CreateBus("host=localhost;persistentMessages=false"))
            {
                bus.Receive<ProductRequestMessage>("customerOrderLine", (message) => {
                    Console.WriteLine(message.ProductName + " requested by " + message.ReplyToCustomer);
                    OrderRequestMessage requestMessage = new OrderRequestMessage
					{
						OrderId = orderNumber,
						ProductId = 1,
						Country = "DK",
                        ReplyToRetailer = message.ReplyToCustomer + orderNumber
					};
                    orderNumber++;
					bus.Publish<OrderRequestMessage>(requestMessage);
					Console.WriteLine("Order request message published");

                    bus.Receive<OrderReplyMessage>(requestMessage.ReplyToRetailer, (response) => {
                        Console.WriteLine("Received warehouse status for product");
                        ProductReplyMessage replyMessage = new ProductReplyMessage()
                        {
                            OrderId = response.OrderId,
                            DaysForDelivery = response.DaysForDelivery,
                            ShippingCharge = response.ShippingCharge
                        };
                        Console.WriteLine("Sending product info to " + message.ReplyToCustomer);
                        bus.Send<ProductReplyMessage>(message.ReplyToCustomer, replyMessage);
                    });
                });

                Console.WriteLine("Waiting for customer order.");
                Console.ReadLine();
            }
        }

        private static void Timeout_Elapsed(object message)
        {
            OrderRequestMessage m = message as OrderRequestMessage;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Timeout on order request " + m.OrderId + " from " + m.Country + "!");
            Console.ResetColor();
        }

        static void HandleOrderReplyMessage(OrderReplyMessage message)
        {
            lock (lockObject)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Reply received from warehouse" + message.WarehouseId);
                Console.WriteLine("Order Id: " + message.OrderId);
                Console.WriteLine("Items in stock: " + message.ItemsInStock);
                Console.WriteLine("Days for delivery: " + message.DaysForDelivery);
                Console.WriteLine("Shipping charge: " + message.ShippingCharge);
                Console.ResetColor();
            }
        }

    }
}
