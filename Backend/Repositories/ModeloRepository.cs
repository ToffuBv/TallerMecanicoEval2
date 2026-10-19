using TallerMecanico.Models;

namespace TallerMecanico.Repositories
{
    public class ModeloRepository : Repository<Modelo>, IModeloRepository
    {
        public ModeloRepository(TallerMecanicoDbContext context) : base(context)
        {
        }
    }
}
