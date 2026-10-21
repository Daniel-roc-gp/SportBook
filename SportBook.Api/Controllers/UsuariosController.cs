
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SportBook.Api.Response;
using SportBook.Core.DTOs;
using SportBook.Core.Entities;
using SportBook.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SportBook.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<UsuariosController> _logger;
        private readonly PasswordHasher<Usuario> _passwordHasher = new();

        public UsuariosController(
            IUsuarioRepository usuarioRepository,
            IMapper mapper,
            ILogger<UsuariosController> logger)
        {
            _usuarioRepository = usuarioRepository;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult> GetUsuarios()
        {
            try
            {
                var usuarios = await _usuarioRepository.GetAllUsuariosAsync();
                var usuariosDto = _mapper.Map<IEnumerable<UsuarioDto>>(usuarios);

                return Ok(new ApiResponse<IEnumerable<UsuarioDto>>(usuariosDto));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al consultar los usuarios.");

                return StatusCode(500,
                    new ApiResponse<string>("Error al consultar los usuarios."));
            }
        }

        [HttpGet("{id:long}")]
        public async Task<ActionResult> GetUsuarioById(long id)
        {
            try
            {
                var usuario = await _usuarioRepository.GetUsuarioByIdAsync(id);

                if (usuario == null)
                {
                    return NotFound(
                        new ApiResponse<string>("Usuario no encontrado."));
                }

                var usuarioDto = _mapper.Map<UsuarioDto>(usuario);

                return Ok(new ApiResponse<UsuarioDto>(usuarioDto));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al consultar el usuario {Id}.", id);

                return StatusCode(500,
                    new ApiResponse<string>("Error al consultar el usuario."));
            }
        }
        [Authorize(Roles = "ADMINISTRADOR")]
        [HttpPost]
        public async Task<ActionResult> InsertUsuario(
    [FromBody] CrearUsuarioAdminDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Nombre) ||
                dto.Nombre.Length > 50 ||
                string.IsNullOrWhiteSpace(dto.Apellido) ||
                dto.Apellido.Length > 50 ||
                string.IsNullOrWhiteSpace(dto.Email) ||
                dto.Email.Length > 100 ||
                string.IsNullOrWhiteSpace(dto.Password) ||
                dto.Password.Length < 8 ||
                dto.Password.Length > 100 ||
                dto.IdRol <= 0 ||
                !new System.ComponentModel.DataAnnotations.EmailAddressAttribute()
                    .IsValid(dto.Email))
            {
                return BadRequest(
                    new ApiResponse<string>("Los datos del usuario no son válidos."));
            }

            try
            {
                if (await _usuarioRepository.ExisteEmailAsync(
                    dto.Email.Trim().ToLowerInvariant()))
                {
                    return Conflict(
                        new ApiResponse<string>("El correo ya está registrado."));
                }

                if (!await _usuarioRepository.RolExisteAsync(dto.IdRol))
                {
                    return BadRequest(
                        new ApiResponse<string>("El rol indicado no existe."));
                }

                var usuario = new Usuario
                {
                    IdRol = dto.IdRol,
                    Nombre = dto.Nombre.Trim(),
                    Apellido = dto.Apellido.Trim(),
                    Email = dto.Email.Trim().ToLowerInvariant(),
                    Telefono = dto.Telefono,
                    Activo = dto.Activo
                };

                usuario.Password = _passwordHasher.HashPassword(
                    usuario, dto.Password);

                await _usuarioRepository.InsertUsuario(usuario);

                var respuesta = _mapper.Map<UsuarioDto>(usuario);

                return StatusCode(201,
                    new ApiResponse<UsuarioDto>(respuesta));
            }
            catch (Exception)
            {
                return StatusCode(
                    500,
                    new ApiResponse<string>("Error al registrar el usuario."));
            }
        }

        [Authorize(Roles = "ADMINISTRADOR")]
        [HttpPut("{id:long}")]
        public async Task<ActionResult> UpdateUsuario(
    long id,
    [FromBody] ActualizarUsuarioDto dto)
        {
            if (id <= 0 ||
                string.IsNullOrWhiteSpace(dto.Nombre) ||
                dto.Nombre.Length > 50 ||
                string.IsNullOrWhiteSpace(dto.Apellido) ||
                dto.Apellido.Length > 50 ||
                string.IsNullOrWhiteSpace(dto.Email) ||
                dto.Email.Length > 100 ||
                !new System.ComponentModel.DataAnnotations.EmailAddressAttribute()
                    .IsValid(dto.Email))
            {
                return BadRequest(
                    new ApiResponse<string>("Los datos del usuario no son válidos."));
            }

            try
            {
                var usuario = await _usuarioRepository.GetUsuarioByIdAsync(id);

                if (usuario == null)
                {
                    return NotFound(
                        new ApiResponse<string>("Usuario no encontrado."));
                }

                var emailNuevo = dto.Email.Trim().ToLowerInvariant();

                if (!string.Equals(usuario.Email, emailNuevo,
                    StringComparison.OrdinalIgnoreCase) &&
                    await _usuarioRepository.ExisteEmailAsync(emailNuevo))
                {
                    return Conflict(
                        new ApiResponse<string>("El correo ya está registrado."));
                }

                usuario.Nombre = dto.Nombre.Trim();
                usuario.Apellido = dto.Apellido.Trim();
                usuario.Email = emailNuevo;
                usuario.Telefono = dto.Telefono;
                usuario.Activo = dto.Activo;

                await _usuarioRepository.UpdateUsuario(usuario);

                return Ok(new ApiResponse<UsuarioDto>(
                    _mapper.Map<UsuarioDto>(usuario)));
            }
            catch (Exception)
            {
                return StatusCode(
                    500,
                    new ApiResponse<string>("Error al actualizar el usuario."));
            }
        }
        [Authorize(Roles = "ADMINISTRADOR")]
        [HttpPatch("{id:long}/rol")]
        public async Task<ActionResult> CambiarRol(
    long id,
    [FromBody] CambiarRolUsuarioDto dto)
        {
            if (id <= 0 || dto.IdRol <= 0)
            {
                return BadRequest(
                    new ApiResponse<string>("Los identificadores no son válidos."));
            }

            try
            {
                var usuario = await _usuarioRepository.GetUsuarioByIdAsync(id);

                if (usuario == null)
                {
                    return NotFound(
                        new ApiResponse<string>("Usuario no encontrado."));
                }

                if (!await _usuarioRepository.RolExisteAsync(dto.IdRol))
                {
                    return BadRequest(
                        new ApiResponse<string>("El rol indicado no existe."));
                }

                usuario.IdRol = dto.IdRol;

                await _usuarioRepository.UpdateUsuario(usuario);

                return Ok(new ApiResponse<UsuarioDto>(
                    _mapper.Map<UsuarioDto>(usuario)));
            }
            catch (Exception)
            {
                return StatusCode(
                    500,
                    new ApiResponse<string>("Error al cambiar el rol."));
            }
        }
    }
}
