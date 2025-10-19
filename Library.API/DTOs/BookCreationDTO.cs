using System.ComponentModel.DataAnnotations;

namespace Library.API.DTOs
{
    // DTOs/BookCreationDTO.cs
    public class BookCreationDTO
    {
        // Campos que el usuario envía al crear un libro
        [Required]
        public string Title { get; set; }

        [Required]
        public string Author { get; set; }

        public int PublicationYear { get; set; }

        public string CoverImageUrl { get; set; } // Opcional
    }

    // DTOs/BookUpdateDTO.cs
    public class BookUpdateDTO
    {
        // Para la edición, todos los campos son opcionales, pero si se envían, se actualizan.
        // Usar '?' para hacerlos anulables si se desea actualización parcial (PATCH/PUT)
        public string? Title { get; set; }
        public string? Author { get; set; }
        public int? PublicationYear { get; set; }
        public string? CoverImageUrl { get; set; }
    }
}
