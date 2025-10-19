namespace Library.API.DTOs
{
    // DTOs/UserDTO.cs (Salida)
    public class UserDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        // NOTA: NO incluir PasswordHash
    }

    // DTOs/BookDTO.cs (Salida)
    public class BookDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public int PublicationYear { get; set; }
        public string CoverImageUrl { get; set; }
        public double AverageRating { get; set; } // Campo calculado, no en la entidad

        // NOTA: Se podría omitir UserId si no es relevante para el cliente
    }
}
