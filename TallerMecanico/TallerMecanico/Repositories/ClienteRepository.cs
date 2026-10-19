using Microsoft.EntityFrameworkCore;
using TallerMecanico.Models;

namespace TallerMecanico.Repositories
{
    public class ClienteRepository : Repository<Cliente>, IClienteRepository
    {
        public ClienteRepository(TallerMecanicoDbContext context) : base(context)
        {
        }

        public async Task<Cliente?> GetClienteByCorreoAsync(string correo)
        {
            return await _dbSet.FirstOrDefaultAsync(c => c.Correo == correo);
        }
    }
}
