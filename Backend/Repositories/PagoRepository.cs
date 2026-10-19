using TallerMecanico.Models;

namespace TallerMecanico.Repositories
{
    public class PagoRepository : Repository<Pago>, IPagoRepository
    {
        public PagoRepository(TallerMecanicoDbContext context) : base(context)
        {
        }
    }
}
