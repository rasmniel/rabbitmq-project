using System;
namespace Customer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Identify yourself");
            string name = Console.ReadLine();

            Console.WriteLine("Where are you from?");
            string country = Console.ReadLine();

            Customer customer = new Customer(name, country);

            Console.WriteLine("What do you want?");
            string product = Console.ReadLine();

            customer.RequestProduct(product);
        }
    }
}
