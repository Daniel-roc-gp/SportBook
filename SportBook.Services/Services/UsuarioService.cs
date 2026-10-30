using SportBook.Core.Entities;
using SportBook.Core.Interfaces;
using SportBook.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportBook.Services.Services
{
    public class UsuarioService : IUsuarioService
    {
        public readonly IBaseRepository<Usuario> _usuarioRepository;
        public readonly IUsuarioRepository _repository;

        public UsuarioService(
            IBaseRepository<Usuario> usuarioRepository,
            IUsuarioRepository repository)
        {
            _usuarioRepository = usuarioRepository;
            _repository = repository;
        }

        public async Task<IEnumerable<Usuario>> GetAllUsuariosAsync()
        {
            return await _repository.GetAllUsuariosAsync();
        }

        public async Task<Usuario?> GetUsuarioByIdAsync(long id)
        {
            return await _repository.GetUsuarioByIdAsync(id);
        }

        public async Task<string> InsertUsuario(Usuario usuario)
        {
            // Verificar que el correo no esté registrado
            if (await _repository.ExisteEmailAsync(usuario.Email))
            {
                return "EMAIL_EXISTS";
            }

            // Verificar que el rol exista
            if (!await _repository.RolExisteAsync(usuario.IdRol))
            {
                return "ROLE_INVALID";
            }

            await _usuarioRepository.Add(usuario);

            return "OK";
        }

        public async Task<string> UpdateUsuario(Usuario usuario)
        {
            // Verificar si el correo pertenece a otro usuario
            var usuarioEmail =
                await _repository.GetUsuarioByEmailAsync(usuario.Email);

            if (usuarioEmail != null &&
                usuarioEmail.IdUsuario != usuario.IdUsuario)
            {
                return "EMAIL_EXISTS";
            }

            await _usuarioRepository.Update(usuario);

            return "OK";
        }

        public async Task<string> CambiarRolUsuarioAsync(
            long id,
            long idRol)
        {
            var usuario =
                await _repository.GetUsuarioByIdAsync(id);

            if (usuario == null)
            {
                return "USER_NOT_FOUND";
            }

            if (!await _repository.RolExisteAsync(idRol))
            {
                return "ROLE_INVALID";
            }

            usuario.IdRol = idRol;

            await _usuarioRepository.Update(usuario);

            return "OK";
        }
    }
}
