

namespace StoreApi.Models
{
    public class Cart
    {
        public int Id { get; set; }
        public string? CustomerId { get; set; }
        public List<CartItem> Items { get; set; } = new List<CartItem>();
        public string? ClientSecret { get; set; }
        public string? PaymentIntentId { get; set; }
        public void AddItem(Product product, int quantity)
        {
            if (Items.All(item => item.ProductId != product.Id))
            {
                Items.Add(new CartItem { Product = product, Quantity = quantity });
            }

            var existingItem = Items.FirstOrDefault(item => item.ProductId == product.Id);
            if (existingItem != null) existingItem.Quantity += quantity;
        }

        public void DeleteItem(int productId, int quantity)
        {
            var existingItem = Items.FirstOrDefault(item => item.ProductId == productId);
            if (existingItem == null) return;
            existingItem.Quantity -= quantity;

            if (existingItem.Quantity <= 0)
            {
                Items.Remove(existingItem);
            }

        }
    }
}
