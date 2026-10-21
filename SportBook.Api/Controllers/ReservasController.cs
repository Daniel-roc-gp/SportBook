
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportBook.Api.Response;
using SportBook.Core.DTOs;
using SportBook.Core.Entities;
using SportBook.Core.Interfaces;
using SportBook.Services.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SportBook.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservasController : ControllerBase
    {
        private readonly IReservaRepository _reservaRepository;
        private readonly IMapper _mapper;
        private readonly CrearReservaDtoValidator _crearReservaValidator;
        private readonly ILogger<ReservasController> _logger;

        public ReservasController(
            IReservaRepository reservaRepository,
            IMapper mapper,
            CrearReservaDtoValidator crearReservaValidator,
            ILogger<ReservasController> logger)
        {
            _reservaRepository = reservaRepository;
            _mapper = mapper;
            _crearReservaValidator = crearReservaValidator;
            _logger = logger;
        }

        private bool EsPersonal()
        {
            return User.IsInRole("ADMINISTRADOR") ||
                   User.IsInRole("ENCARGADO");
        }

        private bool ObtenerIdUsuario(out long idUsuario)
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return long.TryParse(claim, out idUsuario);
        }

        [Authorize(Roles = "ADMINISTRADOR,ENCARGADO")]
        [HttpGet]
        public async Task<ActionResult> GetReservas()
        {
            try
            {
                var reservas =
                    await _reservaRepository.GetAllReservasAsync();

                var reservasDto =
                    _mapper.Map<IEnumerable<ReservaDto>>(reservas);

                return Ok(
                    new ApiResponse<IEnumerable<ReservaDto>>(reservasDto));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al consultar las reservas.");

                return StatusCode(500,
                    new ApiResponse<string>(
                        "Error al consultar las reservas."));
            }
        }

        [Authorize]
        [HttpGet("{id:long}")]
        public async Task<ActionResult> GetReservaById(long id)
        {
            if (id <= 0)
            {
                return BadRequest(
                    new ApiResponse<string>(
                        "El ID debe ser mayor que cero."));
            }

            try
            {
                var reserva =
                    await _reservaRepository.GetReservaByIdAsync(id);

                if (reserva == null)
                {
                    return NotFound(
                        new ApiResponse<string>("Reserva no encontrada."));
                }

                if (!ObtenerIdUsuario(out long idUsuario))
                {
                    return Unauthorized(
                        new ApiResponse<string>(
                            "No se pudo identificar al usuario."));
                }

                if (!EsPersonal() && reserva.IdUsuario != idUsuario)
                {
                    return Forbid();
                }

                var reservaDto = _mapper.Map<ReservaDto>(reserva);

                return Ok(new ApiResponse<ReservaDto>(reservaDto));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error al consultar la reserva {Id}.", id);

                return StatusCode(500,
                    new ApiResponse<string>(
                        "Error al consultar la reserva."));
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> InsertReserva(
            [FromBody] CrearReservaDto reservaDto)
         {
            if (!ObtenerIdUsuario(out long idUsuario))
            {
                return Unauthorized(
                    new ApiResponse<string>(
                        "No se pudo identificar al usuario."));
            }

            if (User.IsInRole("CLIENTE"))
            {
                // El cliente solo puede reservar a su propio nombre.
                reservaDto.IdUsuario = idUsuario;
            }
            else if (!EsPersonal())
            {
                return Forbid();
            }

            // En la base de datos actual, 2 corresponde a CONFIRMADA.
            reservaDto.IdEstado = 2;

            var validationResult =
                await _crearReservaValidator.ValidateAsync(reservaDto);

            if (!validationResult.IsValid)
            {
                return BadRequest(new ApiResponse<object>(new
                {
                    message = "Error de validación",
                    errors = validationResult.Errors.Select(e => new
                    {
                        field = e.PropertyName,
                        error = e.ErrorMessage
                    })
                }));
            }

            try
            {
                if (!await _reservaRepository.UsuarioActivoExisteAsync(
                    reservaDto.IdUsuario))
                {
                    return BadRequest(
                        new ApiResponse<string>(
                            "El usuario no existe o está inactivo."));
                }

                if (!await _reservaRepository.CanchaActivaExisteAsync(
                    reservaDto.IdCancha))
                {
                    return BadRequest(
                        new ApiResponse<string>(
                            "La cancha no existe o está inactiva."));
                }

                if (!await _reservaRepository.HorarioActivoExisteAsync(
                    reservaDto.IdHorario))
                {
                    return BadRequest(
                        new ApiResponse<string>(
                            "El horario no existe o está inactivo."));
                }

                if (!await _reservaRepository.EstadoReservaExisteAsync(
                    reservaDto.IdEstado))
                {
                    return BadRequest(
                        new ApiResponse<string>(
                            "El estado de reserva no existe."));
                }

                if (await _reservaRepository.ExisteReservaEnHorarioAsync(
                    reservaDto.IdCancha,
                    reservaDto.IdHorario,
                    reservaDto.Fecha))
                {
                    return Conflict(
                        new ApiResponse<string>(
                            "La cancha ya está reservada para ese día y horario."));
                }

                var reserva = _mapper.Map<Reserva>(reservaDto);
                reserva.Codigo = Guid.NewGuid().ToString("N")[..20];

                await _reservaRepository.InsertReserva(reserva);

                var respuesta = _mapper.Map<ReservaDto>(reserva);

                return CreatedAtAction(
                    nameof(GetReservaById),
                    new { id = reserva.IdReserva },
                    new ApiResponse<ReservaDto>(respuesta));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al registrar la reserva.");

                return StatusCode(500,
                    new ApiResponse<string>(
                        "Error al registrar la reserva."));
            }

            var capacidad = await _reservaRepository.GetCapacidadCanchaAsync(
    reservaDto.IdCancha);
                
            if (capacidad == null)
            {
                return BadRequest(
                    new ApiResponse<string>("La cancha no existe."));
            }

            if (reservaDto.Jugadores > capacidad.Value)
            {
                return BadRequest(
                    new ApiResponse<string>(
                        "La cantidad de jugadores supera la capacidad de la cancha."));
            }
        }

        [Authorize(Roles = "ADMINISTRADOR,ENCARGADO")]
        [HttpPut("{id:long}")]
        public async Task<ActionResult> UpdateReserva(
            long id,
            [FromBody] ReservaDto reservaDto)
        {
            if (id <= 0 || id != reservaDto.IdReserva)
            {
                return BadRequest(
                    new ApiResponse<string>(
                        "El ID de la ruta debe coincidir con el ID de la reserva."));
            }

            try
            {
                var reservaExistente =
                    await _reservaRepository.GetReservaByIdAsync(id);

                if (reservaExistente == null)
                {
                    return NotFound(
                        new ApiResponse<string>(
                            "Reserva no encontrada."));
                }

                _mapper.Map(reservaDto, reservaExistente);

                await _reservaRepository.UpdateReserva(reservaExistente);

                var respuesta =
                    _mapper.Map<ReservaDto>(reservaExistente);

                return Ok(new ApiResponse<ReservaDto>(respuesta));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error al actualizar la reserva {Id}.", id);

                return StatusCode(500,
                    new ApiResponse<string>(
                        "Error al actualizar la reserva."));
            }
        }

        [Authorize]
        [HttpPatch("{id:long}/cancelar")]
        public async Task<ActionResult> CancelarReserva(long id)
        {
            if (id <= 0)
            {
                return BadRequest(
                    new ApiResponse<string>(
                        "El ID debe ser mayor que cero."));
            }

            try
            {
                if (!ObtenerIdUsuario(out long idUsuario))
                {
                    return Unauthorized(
                        new ApiResponse<string>(
                            "No se pudo identificar al usuario."));
                }

                var reserva =
                    await _reservaRepository.GetReservaByIdAsync(id);

                if (reserva == null)
                {
                    return NotFound(
                        new ApiResponse<string>(
                            "Reserva no encontrada."));
                }

                if (!EsPersonal() && reserva.IdUsuario != idUsuario)
                {
                    return Forbid();
                }

                var resultado =
                    await _reservaRepository.CancelarReservaAsync(id);

                if (resultado == "NOT_FOUND")
                {
                    return NotFound(
                        new ApiResponse<string>(
                            "Reserva no encontrada."));
                }

                if (resultado == "CANCEL_STATE_NOT_FOUND")
                {
                    return BadRequest(
                        new ApiResponse<string>(
                            "No existe un estado de cancelación en la base de datos."));
                }

                if (resultado == "ALREADY_CANCELLED")
                {
                    return Conflict(
                        new ApiResponse<string>(
                            "La reserva ya está cancelada."));
                }

                if (resultado != "OK")
                {
                    return StatusCode(500,
                        new ApiResponse<string>(
                            "No se pudo cancelar la reserva."));
                }

                return Ok(
                    new ApiResponse<string>(
                        "Reserva cancelada correctamente."));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error al cancelar la reserva {Id}.", id);

                return StatusCode(500,
                    new ApiResponse<string>(
                        "Error al cancelar la reserva."));
            }
        }

        [Authorize(Roles = "CLIENTE")]
        [HttpGet("mias")]
        public async Task<ActionResult> GetMisReservas()
        {
            if (!ObtenerIdUsuario(out long idUsuario))
            {
                return Unauthorized(
                    new ApiResponse<string>(
                        "No se pudo identificar al usuario."));
            }

            try
            {
                var reservas =
                    await _reservaRepository.GetReservasPorUsuarioAsync(
                        idUsuario);

                var reservasDto =
                    _mapper.Map<IEnumerable<ReservaDto>>(reservas);

                return Ok(
                    new ApiResponse<IEnumerable<ReservaDto>>(reservasDto));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error al consultar las reservas del usuario.");

                return StatusCode(500,
                    new ApiResponse<string>(
                        "Error al consultar las reservas."));
            }
        }
        [Authorize(Roles = "ADMINISTRADOR,ENCARGADO")]
        [HttpPatch("{id:long}/estado")]
        public async Task<ActionResult> ActualizarEstadoReserva(
    long id,
    [FromBody] ActualizarEstadoReservaDto dto)
        {
            if (id <= 0 || dto.IdEstado <= 0)
            {
                return BadRequest(
                    new ApiResponse<string>("Los identificadores no son válidos."));
            }

            try
            {
                var resultado = await _reservaRepository
                    .ActualizarEstadoReservaAsync(id, dto.IdEstado);

                if (resultado == "NOT_FOUND")
                {
                    return NotFound(
                        new ApiResponse<string>("Reserva no encontrada."));
                }

                if (resultado == "STATE_NOT_FOUND")
                {
                    return BadRequest(
                        new ApiResponse<string>("El estado indicado no existe."));
                }

                if (resultado == "ALREADY_IN_STATE")
                {
                    return Conflict(
                        new ApiResponse<string>("La reserva ya tiene ese estado."));
                }

                if (resultado == "INVALID_TRANSITION")
                {
                    return Conflict(
                        new ApiResponse<string>(
                            "La transición de estado no está permitida."));
                }

                if (resultado != "OK")
                {
                    return StatusCode(
                        500,
                        new ApiResponse<string>("No se pudo actualizar el estado."));
                }

                return Ok(
                    new ApiResponse<string>("Estado de reserva actualizado."));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error al cambiar el estado de la reserva {Id}.", id);

                return StatusCode(
                    500,
                    new ApiResponse<string>("Error al cambiar el estado."));
            }
        }
    }
}
