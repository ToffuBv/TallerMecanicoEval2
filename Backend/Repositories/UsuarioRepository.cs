using Microsoft.EntityFrameworkCore;
using TallerMecanico.Models;

namespace TallerMecanico.Repositories
{
    public class UsuarioRepository : Repository<Usuario>, IUsuarioRepository
    {
        public UsuarioRepository(TallerMecanicoDbContext context) : base(context)
        {
        }

        public async Task<Usuario?> GetUsuarioConRolByNombreAsync(string nombre)
        {
            return await _dbSet
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(u => u.Nombre == nombre);
        }
    }
}
