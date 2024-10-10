namespace DTO
{
    public class Product
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }

        public ICollection<Order> Orders { get; set; }
        public ICollection<Comment> Comments { get; set; }

    }
}
