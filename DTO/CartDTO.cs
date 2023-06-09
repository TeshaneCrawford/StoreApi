namespace StoreApi.DTO
{
    public class CartDTO
    {
        public int Id { get; set; }

        public string CustomerId { get; set; }
        public List<CartItemDTO> Items { get; set; }
        public string PaymentIntentId { get; set; }
        public string ClientSecret { get; set; }
    }
}
