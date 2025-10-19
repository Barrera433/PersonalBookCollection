using Microsoft.AspNetCore.Mvc;
using Library.API.DTOs;
using Library.API.Interfaces;
using Library.API.Models;
using Microsoft.AspNetCore.Authorization;

namespace Library.API.Controllers
{
    // Controllers/BooksController.cs

    [Authorize] // 🔐 Todas las acciones en este controlador requieren un token JWT válido
    [ApiController]
    [Route("api/[controller]")] // La ruta base será /api/books
    public class BooksController : ControllerBase
    {
        private readonly IBookRepository _bookRepository;

        // 1. Inyección de Dependencias (DI)
        public BooksController(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        // -------------------------------------------------------------------
        // A. Lógica para Listar Mi Colección
        // -------------------------------------------------------------------

        [HttpGet] // GET /api/books
        public async Task<IActionResult> GetMyBooks()
        {
            // 2. Obtener el ID del usuario autenticado desde el JWT Token (la clave de seguridad)
            var userIdString = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(userIdString, out int userId))
            {
                // Error si el token no tiene el ID de usuario
                return Unauthorized();
            }

            // 3. Llamar al repositorio para obtener la colección personal
            var books = await _bookRepository.GetBooksByUserIdAsync(userId);

            // 4. Devolver una respuesta HTTP 200 OK con los datos
            // (Nota: es buena práctica mapear de Model a DTO antes de devolver)
            return Ok(books);
        }

        // -------------------------------------------------------------------
        // B. Lógica para Añadir un Libro
        // -------------------------------------------------------------------

        [HttpPost] // POST /api/books
        public async Task<IActionResult> AddBook([FromBody] BookCreationDTO bookDto)
        {
            // 2. Obtener el ID del usuario autenticado
            var userIdString = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdString, out int userId))
            {
                return Unauthorized();
            }

            // 3. Mapear DTO a la entidad Model
            var newBook = new Book
            {
                Title = bookDto.Title,
                Author = bookDto.Author,
                PublicationYear = bookDto.PublicationYear,
                CoverImageUrl = bookDto.CoverImageUrl,
                UserId = userId // Asignar el libro al usuario autenticado
            };

            // 4. Guardar en la base de datos
            var createdBook = await _bookRepository.AddAsync(newBook);

            // 5. Devolver una respuesta HTTP 201 Created
            return CreatedAtAction(nameof(GetMyBooks), new { id = createdBook.Id }, createdBook);
        }

        // -------------------------------------------------------------------
        // C. Lógica para Añadir una Reseña
        // -------------------------------------------------------------------

        [HttpPost("{bookId}/reviews")] // POST /api/books/5/reviews
        public async Task<IActionResult> AddReview(int bookId, [FromBody] ReviewCreationDTO reviewDto)
        {
            var userIdString = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdString, out int userId))
            {
                return Unauthorized();
            }

            // 1. Opcional: Verificar que el libro exista antes de añadir la reseña (no se muestra aquí).

            var newReview = new Review
            {
                Rating = reviewDto.Rating,
                Comment = reviewDto.Comment,
                BookId = bookId, // Clave del libro en la ruta
                UserId = userId, // Clave del usuario autenticado
                DateCreated = DateTime.Now
            };

            var createdReview = await _bookRepository.AddReviewAsync(newReview);

            return Created("", createdReview); // 201 Created
        }
    }
}
