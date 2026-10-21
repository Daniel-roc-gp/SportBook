using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SportBook.Api.Response;
using SportBook.Core.DTOs;
using SportBook.Core.Entities;
using SportBook.Core.Interfaces;
using SportBook.Services.Validators;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SportBook.Api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IUsuarioRepository _repository;
        private readonly RegistrarUsuarioDtoValidator _registroValidator;
        private readonly LoginDtoValidator _loginValidator;
        private readonly IConfiguration _configuration;
        private readonly PasswordHasher<Usuario> _passwordHasher = new();

        public AuthController(
            IUsuarioRepository repository,
            RegistrarUsuarioDtoValidator registroValidator,
            LoginDtoValidator loginValidator,
            IConfiguration configuration)
        {
            _repository = repository;
            _registroValidator = registroValidator;
            _loginValidator = loginValidator;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<ActionResult> Registrar(
            [FromBody] RegistrarUsuarioDto dto)
        {
            var validacion = await _registroValidator.ValidateAsync(dto);

            if (!validacion.IsValid)
            {
                return BadRequest(new ApiResponse<object>(new
                {
                    message = "Error de validación",
                    errors = validacion.Errors.Select(e => new
                    {
                        field = e.PropertyName,
                        error = e.ErrorMessage
                    })
                }));
            }

            if (await _repository.ExisteEmailAsync(dto.Email))
            {
                return Conflict(
                    new ApiResponse<string>("El correo ya está registrado."));
            }

            var rolId = await _repository.GetRolIdPorNombreAsync("CLIENTE");

            if (rolId == null)
            {
                return StatusCode(500,
                    new ApiResponse<string>("No existe el rol CLIENTE."));
            }

            var usuario = new Usuario
            {
                IdRol = rolId.Value,
                Nombre = dto.Nombre.Trim(),
                Apellido = dto.Apellido.Trim(),
                Email = dto.Email.Trim().ToLowerInvariant(),
                Telefono = dto.Telefono,
                Activo = true
            };

            usuario.Password = _passwordHasher.HashPassword(
                usuario, dto.Password);

            await _repository.InsertUsuario(usuario);

            return StatusCode(201, new ApiResponse<string>(
                "Cuenta creada correctamente. Ya puedes iniciar sesión."));
        }

        [HttpPost("login")]
        public async Task<ActionResult> IniciarSesion([FromBody] LoginDto dto)
        {
            var validacion = await _loginValidator.ValidateAsync(dto);

            if (!validacion.IsValid)
            {
                return BadRequest(new ApiResponse<object>(new
                {
                    message = "Error de validación",
                    errors = validacion.Errors.Select(e => new
                    {
                        field = e.PropertyName,
                        error = e.ErrorMessage
                    })
                }));
            }

            var usuario = await _repository.GetUsuarioByEmailAsync(
                dto.Email.Trim().ToLowerInvariant());

            if (usuario == null || usuario.Activo != true)
            {
                return Unauthorized(
                    new ApiResponse<string>("Credenciales incorrectas o cuenta inactiva."));
            }

            var resultado = _passwordHasher.VerifyHashedPassword(
                usuario, usuario.Password, dto.Password);

            if (resultado == PasswordVerificationResult.Failed)
            {
                return Unauthorized(
                    new ApiResponse<string>("Credenciales incorrectas o cuenta inactiva."));
            }

            var key = _configuration["Jwt:Key"];

            if (string.IsNullOrWhiteSpace(key) || Encoding.UTF8.GetByteCount(key) < 32)
            {
                return StatusCode(500,
                    new ApiResponse<string>("La clave JWT no está configurada correctamente."));
            }

            var rol = usuario.IdRolNavigation.Nombre;

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.IdUsuario.ToString()),
                new Claim(ClaimTypes.Name, usuario.Email),
                new Claim(ClaimTypes.Role, rol)
            };

            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));

            var credentials = new SigningCredentials(
                signingKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddHours(4),
                signingCredentials: credentials);

            var respuesta = new LoginRespuestaDto
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                IdUsuario = usuario.IdUsuario,
                Nombre = usuario.Nombre,
                Email = usuario.Email,
                Rol = rol
            };

            return Ok(new ApiResponse<LoginRespuestaDto>(respuesta));
        }
    }
}