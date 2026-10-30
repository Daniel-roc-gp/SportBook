
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SportBook.Api.Response;
using SportBook.Core.DTOs;
using SportBook.Core.Entities;
using SportBook.Services.Interfaces;
using SportBook.Services.Validators;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SportBook.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservasController : ControllerBase
    {
        private readonly IReservaService _reservaService;
        private readonly IMapper _mapper;
        private readonly CrearReservaDtoValidator _crearValidator;

        public ReservasController(
            IMapper mapper,
            IReservaService reservaService,
            CrearReservaDtoValidator crearValidator)
        {
            _reservaService = reservaService;
            _mapper = mapper;
            _crearValidator = crearValidator;
        }

        #region Sin DTOs

        //[HttpGet]
        //public async Task<ActionResult> GetReservas()
        //{
        //    var reservas = await _reservaService.GetAllReservasAsync();
        //    return Ok(reservas);
        //}

        //[HttpGet("{id}")]
        //public async Task<ActionResult> GetReservaById(long id)
        //{
        //    var reserva = await _reservaService.GetReservaByIdAsync(id);
        //    return Ok(reserva);
        //}

        #endregion

        #region Con DTOs

        //[HttpGet("dto")]
        //public async Task<IActionResult> GetReservasDto()
        //{
        //    var reservas = await _reservaService.GetAllReservasAsync();
        //    var reservaDto = _mapper.Map<IEnumerable<ReservaDto>>(reservas);
        //    return Ok(reservaDto);
        //}

        //[HttpGet("dto/{id}")]
        //public async Task<IActionResult> GetReservaByIdDto(long id)
        //{
        //    var reserva = await _reservaService.GetReservaByIdAsync(id);
        //    var reservaDto = _mapper.Map<ReservaDto>(reserva);
        //    return Ok(reservaDto);
        //}

        #endregion

        #region Dto-AutoMapper

        // CONSULTAR TODAS LAS RESERVAS
        [HttpGet("dto/mapper")]
        public async Task<IActionResult> GetReservasDtoMapper()
        {
            var reservas = await _reservaService.GetAllReservasAsync();

            var reservaDto =
                _mapper.Map<IEnumerable<ReservaDto>>(reservas);

            var response =
                new ApiResponse<IEnumerable<ReservaDto>>(reservaDto);

            return Ok(response);
        }

        // CONSULTAR RESERVA POR ID
        [HttpGet("dto/mapper/{id}")]
        public async Task<IActionResult> GetReservaByIdDtoMapper(long id)
        {
            if (id <= 0)
                return BadRequest("El ID debe ser mayor que cero.");

            var reserva = await _reservaService.GetReservaByIdAsync(id);

            if (reserva == null)
                return NotFound("Reserva no encontrada.");

            var reservaDto = _mapper.Map<ReservaDto>(reserva);

            var response = new ApiResponse<ReservaDto>(reservaDto);

            return Ok(response);
        }

        // REGISTRAR RESERVA
        [HttpPost("dto/mapper")]
        public async Task<IActionResult> InsertReservaDtoMapper(
            [FromBody] CrearReservaDto reservaDto)
        {
            // Validar el DTO
            var validationResult =
                await _crearValidator.ValidateAsync(reservaDto);

            if (!validationResult.IsValid)
            {
                return BadRequest(new
                {
                    message = "Error de validación",
                    errors = validationResult.Errors.Select(e => new
                    {
                        field = e.PropertyName,
                        error = e.ErrorMessage
                    })
                });
            }

            // El usuario se indica directamente desde el DTO
            if (reservaDto.IdUsuario <= 0)
            {
                return BadRequest(
                    new ApiResponse<string>(
                        "Debes indicar un usuario válido."));
            }

            // En la base de datos actual, 2 corresponde a CONFIRMADA.
            reservaDto.IdEstado = 2;

            try
            {
                var reserva = _mapper.Map<Reserva>(reservaDto);

                reserva.Codigo =
                    Guid.NewGuid().ToString("N")[..20];

                var resultado =
                    await _reservaService.InsertReserva(reserva);

                switch (resultado)
                {
                    case "USER_INVALID":
                        return BadRequest(
                            new ApiResponse<string>(
                                "El usuario no existe o está inactivo."));

                    case "COURT_INVALID":
                    case "COURT_NOT_FOUND":
                        return BadRequest(
                            new ApiResponse<string>(
                                "La cancha no existe o está inactiva."));

                    case "SCHEDULE_INVALID":
                        return BadRequest(
                            new ApiResponse<string>(
                                "El horario no existe o está inactivo."));

                    case "STATE_INVALID":
                        return BadRequest(
                            new ApiResponse<string>(
                                "El estado de reserva no existe."));

                    case "CAPACITY_EXCEEDED":
                        return BadRequest(
                            new ApiResponse<string>(
                                "La cantidad de jugadores supera la capacidad de la cancha."));

                    case "SLOT_OCCUPIED":
                        return Conflict(
                            new ApiResponse<string>(
                                "La cancha ya está reservada para ese día y horario."));

                    case "OK":

                        var resultDto =
                            _mapper.Map<ReservaDto>(reserva);

                        return CreatedAtAction(
                            nameof(GetReservaByIdDtoMapper),
                            new { id = reserva.IdReserva },
                            resultDto);

                    default:
                        return StatusCode(500,
                            new ApiResponse<string>(
                                "No se pudo registrar la reserva."));
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500,
                    new
                    {
                        message = "Error al registrar la reserva",
                        error = ex.Message
                    });
            }
        }

        // ACTUALIZAR RESERVA
        [HttpPut("dto/mapper/{id}")]
        public async Task<IActionResult> UpdateReservaDtoMapper(
            long id,
            [FromBody] ReservaDto reservaDto)
        {
            if (id != reservaDto.IdReserva)
                return BadRequest("El ID de la reserva no coincide.");

            var reserva =
                await _reservaService.GetReservaByIdAsync(id);

            if (reserva == null)
                return NotFound("Reserva no encontrada.");

            try
            {
                _mapper.Map(reservaDto, reserva);

                await _reservaService.UpdateReserva(reserva);

                var resultDto =
                    _mapper.Map<ReservaDto>(reserva);

                return Ok(resultDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500,
                    new
                    {
                        message = "Error al actualizar la reserva",
                        error = ex.Message
                    });
            }
        }

        // CANCELAR RESERVA
        [HttpPatch("dto/mapper/{id}/cancelar")]
        public async Task<IActionResult> CancelarReservaDtoMapper(long id)
        {
            if (id <= 0)
                return BadRequest("El ID debe ser mayor que cero.");

            try
            {
                var reserva =
                    await _reservaService.GetReservaByIdAsync(id);

                if (reserva == null)
                    return NotFound("Reserva no encontrada.");

                var resultado =
                    await _reservaService.CancelarReservaAsync(id);

                switch (resultado)
                {
                    case "NOT_FOUND":
                        return NotFound("Reserva no encontrada.");

                    case "CANCEL_STATE_NOT_FOUND":
                    case "STATE_NOT_FOUND":
                        return BadRequest(
                            "No existe el estado de cancelación.");

                    case "ALREADY_CANCELLED":
                        return Conflict(
                            "La reserva ya está cancelada.");

                    case "CANNOT_CANCEL_COMPLETED":
                        return Conflict(
                            "No se puede cancelar una reserva completada.");

                    case "OK":
                        return Ok(
                            "Reserva cancelada correctamente.");

                    default:
                        return StatusCode(500,
                            "No se pudo cancelar la reserva.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500,
                    new
                    {
                        message = "Error al cancelar la reserva",
                        error = ex.Message
                    });
            }
        }

        // CONSULTAR RESERVAS POR USUARIO
        [HttpGet("dto/mapper/mias/{idUsuario}")]
        public async Task<IActionResult> GetMisReservasDtoMapper(
            long idUsuario)
        {
            if (idUsuario <= 0)
                return BadRequest("El ID del usuario no es válido.");

            try
            {
                var reservas =
                    await _reservaService.GetReservasPorUsuarioAsync(
                        idUsuario);

                var reservasDto =
                    _mapper.Map<IEnumerable<ReservaDto>>(reservas);

                var response =
                    new ApiResponse<IEnumerable<ReservaDto>>(reservasDto);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500,
                    new
                    {
                        message = "Error al consultar las reservas",
                        error = ex.Message
                    });
            }
        }

        // ACTUALIZAR ESTADO DE RESERVA
        [HttpPatch("dto/mapper/{id}/estado")]
        public async Task<IActionResult> ActualizarEstadoReservaDtoMapper(
            long id,
            [FromBody] ActualizarEstadoReservaDto dto)
        {
            if (id <= 0 || dto.IdEstado <= 0)
                return BadRequest("Los identificadores no son válidos.");

            try
            {
                var resultado =
                    await _reservaService.ActualizarEstadoReservaAsync(
                        id, dto.IdEstado);

                switch (resultado)
                {
                    case "NOT_FOUND":
                        return NotFound("Reserva no encontrada.");

                    case "STATE_NOT_FOUND":
                        return BadRequest(
                            "El estado indicado no existe.");

                    case "ALREADY_IN_STATE":
                        return Conflict(
                            "La reserva ya tiene ese estado.");

                    case "INVALID_TRANSITION":
                        return Conflict(
                            "La transición de estado no está permitida.");

                    case "OK":
                        return Ok(
                            "Estado de reserva actualizado.");

                    default:
                        return StatusCode(500,
                            "No se pudo actualizar el estado.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500,
                    new
                    {
                        message = "Error al cambiar el estado",
                        error = ex.Message
                    });
            }
        }

        #endregion
    }
}
