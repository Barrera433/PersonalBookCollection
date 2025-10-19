using Library.API.Models;

namespace Library.API.Interfaces
{
    // Interfaces/IUserRepository.cs
    public interface IUserRepository
    {
        Task<User> GetByIdAsync(int id);
        Task<IEnumerable<User>> GetAllAsync();
        Task<User> AddAsync(User user);
        Task<bool> UpdateAsync(User user);
        Task<bool> DeleteAsync(int id);
        Task<User> GetByEmailAsync(string email); // Necesario para login/registro
    }

    // Interfaces/IBookRepository.cs
    public interface IBookRepository
    {
        Task<Book> GetByIdAsync(int id);
        Task<IEnumerable<Book>> GetBooksByUserIdAsync(int userId); // Listar la colección del usuario
        Task<Book> AddAsync(Book book);
        Task<bool> UpdateAsync(Book book);
        Task<bool> DeleteAsync(int id);

        // Funcionalidades de Calificación/Reseña
        Task<Review> AddReviewAsync(Review review);
        Task<IEnumerable<Review>> GetReviewsByBookIdAsync(int bookId);
    }
}
