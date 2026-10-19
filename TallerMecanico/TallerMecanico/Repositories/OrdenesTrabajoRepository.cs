using Microsoft.EntityFrameworkCore;
using TallerMecanico.Models;

namespace TallerMecanico.Repositories
{
    public class OrdenesTrabajoRepository : Repository<OrdenesTrabajo>, IOrdenesTrabajoRepository
    {
        public OrdenesTrabajoRepository(TallerMecanicoDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<OrdenesTrabajo>> GetOrdenesConDetallesAsync()
        {
            return await _dbSet
                .Include(o => o.Cliente)
                .Include(o => o.Mecanico)
                .Include(o => o.Vehiculo)
                .Include(o => o.Pago)
                .Include(o => o.DetalleRepuestos)
                .Include(o => o.DetalleServicios)
                .ToListAsync();
        }

        public async Task<OrdenesTrabajo?> GetOrdenConDetallesByIdAsync(int id)
        {
            return await _dbSet
                .Include(o => o.Cliente)
                .Include(o => o.Mecanico)
                .Include(o => o.Vehiculo)
                .Include(o => o.Pago)
                .Include(o => o.DetalleRepuestos)
                .Include(o => o.DetalleServicios)
                .FirstOrDefaultAsync(o => o.OrdenId == id);
        }
    }
}
