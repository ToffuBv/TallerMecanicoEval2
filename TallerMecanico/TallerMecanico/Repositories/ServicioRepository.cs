using TallerMecanico.Models;

namespace TallerMecanico.Repositories
{
    public class ServicioRepository : Repository<Servicio>, IServicioRepository
    {
        public ServicioRepository(TallerMecanicoDbContext context) : base(context)
        {
        }
    }
}
