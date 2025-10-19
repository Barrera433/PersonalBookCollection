using System.ComponentModel.DataAnnotations;

namespace Library.API.DTOs
{
    // DTOs/ReviewCreationDTO.cs
    public class ReviewCreationDTO
    {
        // Calificación de 1 a 5 estrellas
        [Required]
        [Range(1, 5)] // La calificación debe estar entre 1 y 5
        public int Rating { get; set; }

        // Reseña escrita (puede ser opcional)
        public string? Comment { get; set; }
    }
}
