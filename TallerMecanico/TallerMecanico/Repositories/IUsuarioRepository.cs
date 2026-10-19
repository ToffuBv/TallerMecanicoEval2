using TallerMecanico.Models;

namespace TallerMecanico.Repositories
{
    public interface IUsuarioRepository : IRepository<Usuario>
    {
        Task<Usuario?> GetUsuarioConRolByNombreAsync(string nombre);
    }
}
