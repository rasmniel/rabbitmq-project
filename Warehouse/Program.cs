using System;
namespace Warehouse
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter warehouse id:");
            int id = int.Parse(Console.ReadLine());

            Console.WriteLine("Enter warehouse country code:");
            string country = Console.ReadLine();

            Warehouse warehouse = new Warehouse(id, country);
            warehouse.Listen();
        }
    }
}
