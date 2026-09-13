using Bookstore.Models;
using Microsoft.EntityFrameworkCore;

namespace Bookstore.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Payment> Payments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // seed domyślnej kategorii
            modelBuilder.Entity<Category>().HasData(
                new Category
                {
                    CategoryId = 1,
                    Name = "Bez kategorii",
                }
            );
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    UserId = 1,
                    Name = "Test User",
                    Email = "test-user@xyz.com",
                    PasswordHash = "AQAAAAIAAYagAAAAEN9U7byHyGamN3kRb/OSKRVNeKKhpr9fzpROUZZaotSltUD8Vm7y3JDZ3CwJP10Z/w==",
                    Role = "Admin"
                }
                );
            modelBuilder.Entity<Cart>().HasData(
                new Cart
                {
                    CartId = 1,
                    UserId = 1
                }
                );
            modelBuilder.Entity<CartItem>()
                .HasIndex(ci => new { ci.CartId, ci.BookId })
                .IsUnique();

            modelBuilder.Entity<Payment>()
               .HasOne(p => p.Order)
               .WithOne(o => o.Payment)
               .HasForeignKey<Payment>(p => p.OrderId)
               .OnDelete(DeleteBehavior.Cascade);

            // Obsługa współbieżności dla stanu magazynowego książki.
            modelBuilder.Entity<Book>()
                .Property<uint>("xmin")
                .HasColumnType("xid")
                .ValueGeneratedOnAddOrUpdate()
                .IsConcurrencyToken();

            // Książka użyta w zamówieniu nie może zostać usunięta.
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Book)
                .WithMany(b => b.OrderItems)
                .HasForeignKey(oi => oi.BookId)
                .OnDelete(DeleteBehavior.Restrict);

            // Kategoria używana przez książki nie może zostać usunięta.
            modelBuilder.Entity<Book>()
                .HasOne(b => b.Category)
                .WithMany(c => c.Books)
                .HasForeignKey(b => b.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // Pozycje koszyka są usuwane razem z książką.
            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.Book)
                .WithMany(b => b.CartItems)
                .HasForeignKey(ci => ci.BookId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
