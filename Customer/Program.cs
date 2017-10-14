using System;
using EasyNetQ;
using Messages;

namespace Customer
{
    class CustomerProgram
    {
        static string name, product;

        static void Main(string[] args)
        {
            Console.WriteLine("What is your name?");
            name = Console.ReadLine();

            Console.WriteLine("What do you want?");
            product = Console.ReadLine();

            using (var bus = RabbitHutch.CreateBus("host=localhost;persistentMessages=false"))
            {
                ProductRequestMessage requestMessage = new ProductRequestMessage
                {
                    ProductName = product,
                    ReplyToCustomer = name
                };

                bus.Send<ProductRequestMessage>("customerOrderLine", requestMessage);
                Console.WriteLine("Order request message published for product " + requestMessage.ProductName);

                bus.Receive<ProductReplyMessage>(name, message => handleOrderReply(message));

                Console.ReadLine();
            }
        }

        static void handleOrderReply(ProductReplyMessage message)
        {
            Console.WriteLine("Order Id: " + message.OrderId);
            Console.WriteLine("Days for delivery: " + message.DaysForDelivery);
            Console.WriteLine("Shipping charge: " + message.ShippingCharge);
        }
    }
}
