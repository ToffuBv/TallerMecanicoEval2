using TallerMecanico.Models;

namespace TallerMecanico.Repositories
{
    public class MarcaRepository : Repository<Marca>, IMarcaRepository
    {
        public MarcaRepository(TallerMecanicoDbContext context) : base(context)
        {
        }
    }
}
