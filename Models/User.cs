namespace Bookstore.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public string Role { get; set; } = "Client";

        public Cart Cart { get; set; } = null!;
        public ICollection<Order> Orders { get; set; } = new List<Order>();

    }
}
