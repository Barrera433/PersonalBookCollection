using System.ComponentModel.DataAnnotations;

namespace Library.API.DTOs
{
    // DTOs/UserRegistrationDTO.cs
    public class UserRegistrationDTO
    {
        // Solo los campos necesarios para crear una cuenta
        [Required] // Atributo de validación de ASP.NET Core
        public string Name { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        [EmailAddress] // Atributo para verificar formato de email
        public string Email { get; set; }

        [Required]
        [MinLength(6)] // Requisito de longitud mínima para la contraseña
        public string Password { get; set; }
    }

    // DTOs/UserLoginDTO.cs
    public class UserLoginDTO
    {
        // Solo los campos necesarios para iniciar sesión
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
