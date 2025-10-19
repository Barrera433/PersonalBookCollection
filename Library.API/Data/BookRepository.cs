namespace Library.API.Data
{
    using Library.API.Interfaces;
    using Library.API.Models;
    using Microsoft.EntityFrameworkCore;

    // Data/BookRepository.cs


    public class BookRepository : IBookRepository
    {
        private readonly AppDbContext _context;

        public BookRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Book> GetByIdAsync(int id)
        {
            // Incluye el usuario propietario si fuera necesario, para el control de acceso
            return await _context.Books
            .Include(b => b.User)
            .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<IEnumerable<Book>> GetBooksByUserIdAsync(int userId)
        {
            // Filtra los libros donde la clave foránea UserId coincide con el ID del usuario
            return await _context.Books
                                 .Where(b => b.UserId == userId)
                                 .ToListAsync();
        }

        public async Task<Book> AddAsync(Book book)
        {
            _context.Books.Add(book);
            await _context.SaveChangesAsync();
            return book;
        }

      
        public async Task<bool> UpdateAsync(Book book)
        {
            // Marca la entidad como modificada. Se espera que el objeto 'book' 
            // ya tenga el ID correcto y las propiedades actualizadas.
            _context.Entry(book).State = EntityState.Modified;

            try
            {
                // Guarda los cambios. Retorna true si se afectó al menos una fila.
                // Es crucial que el controlador verifique antes que este libro
                // pertenezca al usuario autenticado.
                return await _context.SaveChangesAsync() > 0;
            }
            catch (DbUpdateConcurrencyException)
            {
                // Si el libro no existe en la DB, fallará la concurrencia
                if (!_context.Books.Any(e => e.Id == book.Id))
                {
                    return false; // No se encontró el libro para actualizar
                }
                throw; // Lanza cualquier otro error
            }
        }

        
        public async Task<bool> DeleteAsync(int id)
        {
            var book = await _context.Books.FindAsync(id);

            if (book == null)
            {
                return false;
            }

            // Remueve la entidad del DbSet
            _context.Books.Remove(book);

            // Guarda los cambios. EF Core manejará la eliminación en cascada 
            // de las Reviews asociadas (si está configurado en el DbContext).
            return await _context.SaveChangesAsync() > 0;
        }

        // Funcionalidad: Añadir una Calificación/Reseña
        public async Task<Review> AddReviewAsync(Review review)
        {
            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();
            return review;
        }

        // Funcionalidad: Obtener Reseñas de un Libro
        public async Task<IEnumerable<Review>> GetReviewsByBookIdAsync(int bookId)
        {
            return await _context.Reviews
                                 .Where(r => r.BookId == bookId)
                                 .Include(r => r.User)
                                 .OrderByDescending(r => r.DateCreated)
                                 .ToListAsync();
        }
    }
}
