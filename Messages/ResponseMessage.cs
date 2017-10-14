namespace Messages
{
    public class ProductResponse
    {
        public int OrderId { get; set; }
        public int DaysForDelivery { get; set; }
        public decimal ShippingCharge { get; set; }
    }

    public class OrderResponse : ProductResponse
    {
        public int WarehouseId { get; set; }
        public int Stock { get; set; }
    }
}
