namespace Library.API.Models
{
    // Models/User.cs
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; } // Debería ser único
        public string PasswordHash { get; set; } // ¡Almacenar siempre la contraseña hasheada!

        // Relación: Un usuario puede tener muchos libros
        public ICollection<Book> Books { get; set; }
        // Relación: Un usuario puede tener muchas reseñas/calificaciones
        public ICollection<Review> Reviews { get; set; }
    }

    // Models/Book.cs
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public int PublicationYear { get; set; }
        public string CoverImageUrl { get; set; } // URL a la imagen

        // Clave Foránea (Foreign Key) para saber a qué usuario pertenece
        public int UserId { get; set; }
        public User User { get; set; } // Propiedad de navegación

        // Relación: Un libro puede tener muchas reseñas/calificaciones
        public ICollection<Review> Reviews { get; set; }
    }

    // Models/Review.cs
    public class Review
    {
        public int Id { get; set; }
        public int Rating { get; set; } // Calificación de 1 a 5
        public string Comment { get; set; }
        public DateTime DateCreated { get; set; }

        // Claves Foráneas
        public int BookId { get; set; }
        public Book Book { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }
    }
}
