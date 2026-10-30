using SportBook.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportBook.Services.Interfaces
{
    public interface IUsuarioService
    {
        Task<IEnumerable<Usuario>> GetAllUsuariosAsync();
        Task<Usuario?> GetUsuarioByIdAsync(long id);
        Task<string> InsertUsuario(Usuario usuario);
        Task<string> UpdateUsuario(Usuario usuario);
        Task<string> CambiarRolUsuarioAsync(long id, long idRol);
    }
}
