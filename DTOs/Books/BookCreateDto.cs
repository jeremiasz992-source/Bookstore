using System.ComponentModel.DataAnnotations;

namespace Bookstore.DTOs.Books
{
    public class BookCreateDto
    {
        [Required(ErrorMessage = "Tytuł książki jest wymagany.")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Autor książki jest wymagany.")]
        public string Author { get; set; } = string.Empty;

        [Required(ErrorMessage = "Wydawnictwo jest wymagane.")]
        public string Publisher { get; set; } = string.Empty;

        [MaxLength(2000, ErrorMessage = "Opis książki może mieć maksymalnie 2000 znaków.")]
        public string Description { get; set; } = string.Empty;

        [MaxLength(500, ErrorMessage = "Adres okładki może mieć maksymalnie 500 znaków.")]
        public string CoverImageUrl { get; set; } = string.Empty;

        [Range(1450, 2100, ErrorMessage = "Rok wydania musi mieścić się w przedziale od 1450 do 2100.")]
        public int Year { get; set; }

        [Range(0.01, 10000, ErrorMessage = "Cena musi mieścić się w przedziale od 0,01 do 10 000 zł.")]
        public decimal Price { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Stan magazynowy nie może być ujemny.")]
        public int Stock { get; set; }

        public int CategoryId { get; set; } = 1; // domyślnie kategoria "Bez kategorii"
    }
}
