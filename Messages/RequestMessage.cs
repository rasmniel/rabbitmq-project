namespace Messages
{
    public class ProductRequest
    {
        public string Sender { get; set; }
        public string Country { get; set; }
        public string Product { get; set; }
    }

    public class OrderRequest : ProductRequest
    {
        public int OrderId { get; set; }
        public int ProductId { get; set; }
    }
}
