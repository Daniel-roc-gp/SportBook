
using SportBook.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SportBook.Core.Interfaces
{
    public interface IUsuarioRepository
    {
        Task<IEnumerable<Usuario>> GetAllUsuariosAsync();

        Task<Usuario?> GetUsuarioByIdAsync(long id);

        Task InsertUsuario(Usuario usuario);

        Task UpdateUsuario(Usuario usuario);

        Task<Usuario?> GetUsuarioByEmailAsync(string email);

        Task<bool> ExisteEmailAsync(string email);

        Task<long?> GetRolIdPorNombreAsync(string nombre);

        Task<bool> RolExisteAsync(long idRol);
    }
}
