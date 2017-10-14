namespace Messages
{
	public class ProductReplyMessage
	{
		public int OrderId { get; set; }
		public int DaysForDelivery { get; set; }
		public decimal ShippingCharge { get; set; }
	}
}
