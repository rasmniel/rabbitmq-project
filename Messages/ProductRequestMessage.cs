namespace Messages
{
    public class ProductRequestMessage
    {
        public string Sender { get; set; }
		public string Country { get; set; }
        public string Product { get; set; }
    }





    public class OrderRequestMessage
    {
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public string Country { get; set; }
        public string ReplyToRetailer { get; set; }
    }
}
