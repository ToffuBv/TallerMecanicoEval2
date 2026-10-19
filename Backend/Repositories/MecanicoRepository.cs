using TallerMecanico.Models;

namespace TallerMecanico.Repositories
{
    public class MecanicoRepository : Repository<Mecanico>, IMecanicoRepository
    {
        public MecanicoRepository(TallerMecanicoDbContext context) : base(context)
        {
        }
    }
}
