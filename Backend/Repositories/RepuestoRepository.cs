using TallerMecanico.Models;

namespace TallerMecanico.Repositories
{
    public class RepuestoRepository : Repository<Repuesto>, IRepuestoRepository
    {
        public RepuestoRepository(TallerMecanicoDbContext context) : base(context)
        {
        }
    }
}
