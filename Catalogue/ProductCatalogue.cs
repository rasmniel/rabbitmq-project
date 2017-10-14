using System.Collections.Generic;

namespace Catalogue
{
    public class ProductCatalogue : Dictionary<string, Product>
    {
        public ProductCatalogue(int stock)
        {
            Add("shovel", new Product()
            {
                ID = 1,
                Stock = stock,
                Price = 100
            });
            Add("chair", new Product()
            {
                ID = 2,
                Stock = stock,
                Price = 250
            });
            Add("vase", new Product()
            {
                ID = 3,
                Stock = stock,
                Price = 800
            });
        }

        public Product getProductById(int productId)
        {
            foreach (string name in Keys)
            {
                Product p = this[name];
                if (p.ID == productId)
                    return p;
            }
            return null;
        }
    }

    public class Product
    {
        public int ID { get; set; }
        public int Stock { get; set; }
        public decimal Price { get; set; }
    }
}
