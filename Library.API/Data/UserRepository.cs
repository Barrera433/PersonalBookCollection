namespace Library.API.Data
{
    using Library.API.Interfaces;
    using Library.API.Models;
    using Microsoft.EntityFrameworkCore;

    // Data/UserRepository.cs


    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User> GetByIdAsync(int id)
        {
            // Usa FindAsync para buscar por clave primaria (más eficiente)
            return await _context.Users.FindAsync(id);
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            // Devuelve todos los usuarios
            return await _context.Users.ToListAsync();
        }

        public async Task<User> AddAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<bool> UpdateAsync(User user)
        {
            // 1. Marca la entidad como modificada
            _context.Entry(user).State = EntityState.Modified;

            try
            {
                // 2. Guarda los cambios. Retorna true si se afectó al menos una fila.
                return await _context.SaveChangesAsync() > 0;
            }
            catch (DbUpdateConcurrencyException)
            {
                // Si el usuario no existe en la DB, fallará la concurrencia
                if (!_context.Users.Any(e => e.Id == user.Id))
                {
                    return false; // No se encontró el usuario para actualizar
                }
                throw; // Lanza cualquier otro error
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return false;
            }

            _context.Users.Remove(user);
            // Retorna true si se afectó al menos una fila
            return await _context.SaveChangesAsync() > 0;
        }

        // Necesario para la autenticación (Login/Registro)
        public async Task<User> GetByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }
    }
}
