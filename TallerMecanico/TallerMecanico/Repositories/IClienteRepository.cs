using TallerMecanico.Models;

namespace TallerMecanico.Repositories
{
    public interface IClienteRepository : IRepository<Cliente>
    {
        // Métodos específicos para Cliente pueden ir aquí, ej:
        Task<Cliente?> GetClienteByCorreoAsync(string correo);
    }
}
