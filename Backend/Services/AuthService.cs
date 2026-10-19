using Microsoft.AspNetCore.Identity;
using TallerMecanico.Models;

namespace TallerMecanico.Services
{
    public class AuthService
    {
        private readonly PasswordHasher<Usuario> hasher = new();
        public string HashPassword(Usuario usuario, string password)
        {
            return hasher.HashPassword(usuario, password);
        }
        public PasswordVerificationResult VerifyPassowrd(Usuario usuario, string passwordIntento)
        {
            return hasher.VerifyHashedPassword(usuario, usuario.PasswordHash, passwordIntento);
        }
    }
}
