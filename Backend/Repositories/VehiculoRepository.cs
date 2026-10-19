using TallerMecanico.Models;

namespace TallerMecanico.Repositories
{
    public class VehiculoRepository : Repository<Vehiculo>, IVehiculoRepository
    {
        public VehiculoRepository(TallerMecanicoDbContext context) : base(context)
        {
        }
    }
}
