
using Microsoft.EntityFrameworkCore;
using SportBook.Core.Entities;
using SportBook.Core.Interfaces;
using SportBook.Infrastructure.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SportBook.Infrastructure.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly SportBookDbContext _sportBookContext;

        public UsuarioRepository(SportBookDbContext sportBookContext)
        {
            _sportBookContext = sportBookContext;
        }

        public async Task<IEnumerable<Usuario>> GetAllUsuariosAsync()
        {
            var usuarios = await _sportBookContext.Usuarios.ToListAsync();
            return usuarios;
        }

        public async Task<Usuario?> GetUsuarioByIdAsync(long id)
        {
            var usuario = await _sportBookContext.Usuarios
                .FirstOrDefaultAsync(x => x.IdUsuario == id);

            return usuario;
        }

        public async Task InsertUsuario(Usuario usuario)
        {
            _sportBookContext.Usuarios.Add(usuario);
            await _sportBookContext.SaveChangesAsync();
        }

        public async Task UpdateUsuario(Usuario usuario)
        {
            _sportBookContext.Usuarios.Update(usuario);
            await _sportBookContext.SaveChangesAsync();
        }

        public async Task<Usuario?> GetUsuarioByEmailAsync(string email)
        {
            return await _sportBookContext.Usuarios
                .Include(x => x.IdRolNavigation)
                .FirstOrDefaultAsync(x => x.Email == email);
        }

        public async Task<bool> ExisteEmailAsync(string email)
        {
            return await _sportBookContext.Usuarios
                .AnyAsync(x => x.Email == email);
        }

        public async Task<long?> GetRolIdPorNombreAsync(string nombre)
        {
            var rol = await _sportBookContext.Rols
                .FirstOrDefaultAsync(x => x.Nombre == nombre);

            return rol?.IdRol;
        }

        public async Task<bool> RolExisteAsync(long idRol)
        {
            return await _sportBookContext.Rols
                .AnyAsync(x => x.IdRol == idRol);
        }
    }
}
