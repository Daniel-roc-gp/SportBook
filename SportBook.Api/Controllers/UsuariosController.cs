
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SportBook.Api.Response;
using SportBook.Core.DTOs;
using SportBook.Core.Entities;
using SportBook.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SportBook.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;
        private readonly IMapper _mapper;
        private readonly ILogger<UsuariosController> _logger;
        private readonly PasswordHasher<Usuario> _passwordHasher = new();

        public UsuariosController(
            IUsuarioService usuarioService,
            IMapper mapper,
            ILogger<UsuariosController> logger)
        {
            _usuarioService = usuarioService;
            _mapper = mapper;
            _logger = logger;
        }

        // CONSULTAR TODOS LOS USUARIOS
        [HttpGet]
        public async Task<ActionResult> GetUsuarios()
        {
            try
            {
                var usuarios =
                    await _usuarioService.GetAllUsuariosAsync();

                var usuariosDto =
                    _mapper.Map<IEnumerable<UsuarioDto>>(usuarios);

                return Ok(
                    new ApiResponse<IEnumerable<UsuarioDto>>(usuariosDto));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error al consultar los usuarios.");

                return StatusCode(500,
                    new ApiResponse<string>(
                        "Error al consultar los usuarios."));
            }
        }

        // CONSULTAR USUARIO POR ID
        [HttpGet("{id:long}")]
        public async Task<ActionResult> GetUsuarioById(long id)
        {
            if (id <= 0)
            {
                return BadRequest(
                    new ApiResponse<string>(
                        "El ID debe ser mayor que cero."));
            }

            try
            {
                var usuario =
                    await _usuarioService.GetUsuarioByIdAsync(id);

                if (usuario == null)
                {
                    return NotFound(
                        new ApiResponse<string>(
                            "Usuario no encontrado."));
                }

                var usuarioDto = _mapper.Map<UsuarioDto>(usuario);

                return Ok(new ApiResponse<UsuarioDto>(usuarioDto));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error al consultar el usuario {Id}.", id);

                return StatusCode(500,
                    new ApiResponse<string>(
                        "Error al consultar el usuario."));
            }
        }

        // REGISTRAR USUARIO
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
                !new System.ComponentModel.DataAnnotations
                    .EmailAddressAttribute().IsValid(dto.Email))
            {
                return BadRequest(
                    new ApiResponse<string>(
                        "Los datos del usuario no son válidos."));
            }

            try
            {
                var usuario = new Usuario
                {
                    IdRol = dto.IdRol,
                    Nombre = dto.Nombre.Trim(),
                    Apellido = dto.Apellido.Trim(),
                    Email = dto.Email.Trim().ToLowerInvariant(),
                    Telefono = dto.Telefono,
                    Activo = dto.Activo
                };

                // Encriptar la contraseña
                usuario.Password = _passwordHasher.HashPassword(
                    usuario, dto.Password);

                var resultado =
                    await _usuarioService.InsertUsuario(usuario);

                if (resultado == "EMAIL_EXISTS")
                {
                    return Conflict(
                        new ApiResponse<string>(
                            "El correo ya está registrado."));
                }

                if (resultado == "ROLE_INVALID")
                {
                    return BadRequest(
                        new ApiResponse<string>(
                            "El rol indicado no existe."));
                }

                var respuesta = _mapper.Map<UsuarioDto>(usuario);

                return StatusCode(201,
                    new ApiResponse<UsuarioDto>(respuesta));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error al registrar el usuario.");

                return StatusCode(500,
                    new ApiResponse<string>(
                        "Error al registrar el usuario."));
            }
        }

        // ACTUALIZAR USUARIO
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
                !new System.ComponentModel.DataAnnotations
                    .EmailAddressAttribute().IsValid(dto.Email))
            {
                return BadRequest(
                    new ApiResponse<string>(
                        "Los datos del usuario no son válidos."));
            }

            try
            {
                var usuario =
                    await _usuarioService.GetUsuarioByIdAsync(id);

                if (usuario == null)
                {
                    return NotFound(
                        new ApiResponse<string>(
                            "Usuario no encontrado."));
                }

                usuario.Nombre = dto.Nombre.Trim();
                usuario.Apellido = dto.Apellido.Trim();
                usuario.Email = dto.Email.Trim().ToLowerInvariant();
                usuario.Telefono = dto.Telefono;
                usuario.Activo = dto.Activo;

                var resultado =
                    await _usuarioService.UpdateUsuario(usuario);

                if (resultado == "EMAIL_EXISTS")
                {
                    return Conflict(
                        new ApiResponse<string>(
                            "El correo ya está registrado."));
                }

                return Ok(new ApiResponse<UsuarioDto>(
                    _mapper.Map<UsuarioDto>(usuario)));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error al actualizar el usuario {Id}.", id);

                return StatusCode(500,
                    new ApiResponse<string>(
                        "Error al actualizar el usuario."));
            }
        }

        // CAMBIAR ROL DE USUARIO
        [HttpPatch("{id:long}/rol")]
        public async Task<ActionResult> CambiarRol(
            long id,
            [FromBody] CambiarRolUsuarioDto dto)
        {
            if (id <= 0 || dto.IdRol <= 0)
            {
                return BadRequest(
                    new ApiResponse<string>(
                        "Los identificadores no son válidos."));
            }

            try
            {
                var resultado =
                    await _usuarioService.CambiarRolUsuarioAsync(
                        id, dto.IdRol);

                if (resultado == "USER_NOT_FOUND")
                {
                    return NotFound(
                        new ApiResponse<string>(
                            "Usuario no encontrado."));
                }

                if (resultado == "ROLE_INVALID")
                {
                    return BadRequest(
                        new ApiResponse<string>(
                            "El rol indicado no existe."));
                }

                var usuario =
                    await _usuarioService.GetUsuarioByIdAsync(id);

                return Ok(new ApiResponse<UsuarioDto>(
                    _mapper.Map<UsuarioDto>(usuario)));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error al cambiar el rol del usuario {Id}.", id);

                return StatusCode(500,
                    new ApiResponse<string>(
                        "Error al cambiar el rol."));
            }
        }
    }
}
