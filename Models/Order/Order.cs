namespace StoreApi.Models.Order
{
    public class Order
    {
        public int Id { get; set; }
        public string? BuyerId { get; set; }
        public ShippingAddress? ShippingAddress { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.Now;
        public List<OrderItem>? OrderItems { get; set; }
        public long SubTotal { get; set; }
        public long DeliveryFee { get; set; }
        public OrderStatus OrderStatus { get; set; } = OrderStatus.Pending;
        public string? PaymentIntentId { get; set; }
        public string? CustomerId { get; internal set; }

        public long GetTotal()
        {
            return SubTotal + DeliveryFee;
        }
    }
}
