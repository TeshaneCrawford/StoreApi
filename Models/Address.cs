namespace StoreApi.Models
{
    public class Address
    {
        public int Id { get; set; }
        public required string Street { get; set; }
        public int Number { get; set; }
        public required string Address2 { get; set; }
        public required string City { get; set; }
        public required string State { get; set; }
        public required string Zip { get; set; }
        public required string Country { get; set; }
    }
}
