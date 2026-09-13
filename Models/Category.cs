namespace Bookstore.Models
{
    public class Category
    {
        public int CategoryId { get; set; }
        public string Name { get; set; } = null!;
        public ICollection<Book> Books { get; set; } = new List<Book>();
    }
}
