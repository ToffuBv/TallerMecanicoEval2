using TallerMecanico.Models;

namespace TallerMecanico.Repositories
{
    public interface IOrdenesTrabajoRepository : IRepository<OrdenesTrabajo>
    {
        Task<IEnumerable<OrdenesTrabajo>> GetOrdenesConDetallesAsync();
        Task<OrdenesTrabajo?> GetOrdenConDetallesByIdAsync(int id);
    }
}
