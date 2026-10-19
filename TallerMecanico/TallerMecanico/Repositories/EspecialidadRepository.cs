using TallerMecanico.Models;

namespace TallerMecanico.Repositories
{
    public class EspecialidadRepository : Repository<Especialidade>, IEspecialidadRepository
    {
        public EspecialidadRepository(TallerMecanicoDbContext context) : base(context)
        {
        }
    }
}
