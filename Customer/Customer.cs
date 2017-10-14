using System;
using EasyNetQ;
using Messages;

namespace Customer
{
    public class Customer
    {
        private string name, country;

        public Customer(string name, string country)
        {
            this.name = name;
            this.country = country;
        }

        public void RequestProduct(string product)
        {
            using (IBus bus = RabbitHutch.CreateBus("host=localhost;persistentMessages=false"))
            {
                ProductRequest requestMessage = getProductRequest(product);
                Console.WriteLine("Order request message published for product " + requestMessage.Product);
                bus.Send("customer_product_request", requestMessage);

                // On incoming reply to order request.
                bus.Receive<ProductResponse>(name, (message) =>
                {
                    handleProductResponse(message);
                });

                Console.ReadLine();
            }
        }

        private ProductRequest getProductRequest(string product)
        {
            // Create and send request message for the specified product.
            return new ProductRequest
            {
                Product = product,
                Country = country,
                Sender = name
            };
        }

        private void handleProductResponse(ProductResponse message)
        {
            lock (this)
            {
                Console.WriteLine("Order Id: " + message.OrderId);
                Console.WriteLine("Days for delivery: " + message.DaysForDelivery);
                Console.WriteLine("Shipping charge: " + message.ShippingCharge);
            }
        }
    }
}
