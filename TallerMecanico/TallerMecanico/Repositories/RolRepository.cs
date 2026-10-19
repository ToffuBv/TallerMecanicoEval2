using TallerMecanico.Models;

namespace TallerMecanico.Repositories
{
    public class RolRepository : Repository<Role>, IRolRepository
    {
        public RolRepository(TallerMecanicoDbContext context) : base(context)
        {
        }
    }
}
