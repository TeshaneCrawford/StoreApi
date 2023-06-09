namespace StoreApi.Models
{
    public class UserAddress : Address
    {
        public new int Id { get; set; }
        public int UserId { get; set; }
        public User? User { get; set; }
    }
}
