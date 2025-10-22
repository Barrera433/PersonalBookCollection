using Microsoft.AspNetCore.Mvc;
using Library.API.DTOs;
using Library.API.Interfaces;
using Library.API.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims; // Necesario para ClaimTypes

namespace Library.API.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    // Controllers/BooksController.cs


    [Authorize] // Todas las acciones requieren un JWT válido
    [ApiController]
    [Route("api/[controller]")] // /api/books
    public class BooksController : ControllerBase
    {
        private readonly IBookRepository _bookRepository;

        public BooksController(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        // Método de utilidad para obtener el UserId del token
        private int GetUserId()
        {
            // Usa NameIdentifier (o el ClaimType que uses para el ID del usuario)
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (int.TryParse(userIdString, out int userId))
            {
                return userId;
            }
            
            return 0;
        }

        
        [HttpGet]
        public async Task<IActionResult> GetMyBooks()
        {
            var userId = GetUserId();
            if (userId == 0) return Unauthorized();

            var books = await _bookRepository.GetBooksByUserIdAsync(userId);

            // Retorna HTTP 200 OK con la lista de libros
            return Ok(books);
        }

        

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBook(int id)
        {
            var book = await _bookRepository.GetByIdAsync(id);

            if (book == null)
            {
                return NotFound(); // HTTP 404
            }

            // **VERIFICACIÓN DE PROPIEDAD (SEGURIDAD)**
            if (book.UserId != GetUserId())
            {
                return Forbid(); // HTTP 403. El libro existe, pero no pertenece al usuario.
            }

            return Ok(book);
        }

     

        [HttpPost]
        public async Task<IActionResult> AddBook([FromBody] BookCreationDTO bookDto)
        {
            var userId = GetUserId();
            if (userId == 0) return Unauthorized();

            var newBook = new Book
            {
                Title = bookDto.Title,
                Author = bookDto.Author,
                PublicationYear = bookDto.PublicationYear,
                CoverImageUrl = bookDto.CoverImageUrl,
                UserId = userId // Asignar al usuario autenticado
            };

            var createdBook = await _bookRepository.AddAsync(newBook);

            // Retorna HTTP 201 Created
            return CreatedAtAction(nameof(GetBook), new { id = createdBook.Id }, createdBook);
        }

      

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBook(int id, [FromBody] BookUpdateDTO bookDto)
        {
            var userId = GetUserId();
            if (userId == 0) return Unauthorized();

            var existingBook = await _bookRepository.GetByIdAsync(id);

            if (existingBook == null)
            {
                return NotFound(); // HTTP 404
            }

            // **VERIFICACIÓN DE PROPIEDAD (SEGURIDAD)**
            if (existingBook.UserId != userId)
            {
                return Forbid(); // HTTP 403
            }

            // Mapear DTO a la entidad existente (solo actualiza si el campo no es nulo)
            existingBook.Title = bookDto.Title ?? existingBook.Title;
            existingBook.Author = bookDto.Author ?? existingBook.Author;

            if (bookDto.PublicationYear.HasValue)
            {
                existingBook.PublicationYear = bookDto.PublicationYear.Value;
            }
            existingBook.CoverImageUrl = bookDto.CoverImageUrl ?? existingBook.CoverImageUrl;

            bool success = await _bookRepository.UpdateAsync(existingBook);

            if (success)
            {
                return NoContent(); // HTTP 204: Petición exitosa, sin contenido para devolver
            }

            return StatusCode(500, "Error al actualizar el libro en la base de datos.");
        }

        

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var userId = GetUserId();
            if (userId == 0) return Unauthorized();

            var existingBook = await _bookRepository.GetByIdAsync(id);

            if (existingBook == null)
            {
                return NotFound(); // HTTP 404
            }

            // **VERIFICACIÓN DE PROPIEDAD (SEGURIDAD)**
            if (existingBook.UserId != userId)
            {
                return Forbid(); // HTTP 403
            }

            bool success = await _bookRepository.DeleteAsync(id);

            if (success)
            {
                return NoContent(); // HTTP 204: Eliminación exitosa, sin contenido
            }

            return StatusCode(500, "Error al eliminar el libro en la base de datos.");
        }

       
        [HttpPost("{bookId}/reviews")]
        public async Task<IActionResult> AddReview(int bookId, [FromBody] ReviewCreationDTO reviewDto)
        {
            var userId = GetUserId();
            if (userId == 0) return Unauthorized();

            // Opcional: Verificar si el libro existe y si pertenece al usuario (aunque la reseña no necesita ser propia)
            var book = await _bookRepository.GetByIdAsync(bookId);
            if (book == null)
            {
                return NotFound($"Libro con ID {bookId} no encontrado.");
            }

            var newReview = new Review
            {
                Rating = reviewDto.Rating,
                Comment = reviewDto.Comment,
                BookId = bookId,
                UserId = userId,
                DateCreated = DateTime.Now
            };

            var createdReview = await _bookRepository.AddReviewAsync(newReview);

            // Retorna HTTP 201 Created
            return Created("", createdReview);
        }

        // -------------------------------------------------------------------
        // NUEVO: Lógica para Obtener Reseñas (GET /api/books/{bookId}/reviews)
        // -------------------------------------------------------------------

        [HttpGet("{bookId}/reviews")]
        [AllowAnonymous] // Permitir ver las reseñas sin autenticación (opcional, ajusta según necesidad)
        public async Task<IActionResult> GetReviews(int bookId)
        {
            var reviews = await _bookRepository.GetReviewsByBookIdAsync(bookId);

            if (!reviews.Any())
            {
                return NotFound($"No se encontraron reseñas para el libro con ID {bookId}.");
            }

            return Ok(reviews);
        }
    }
}
