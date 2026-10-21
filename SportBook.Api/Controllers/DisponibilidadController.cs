
using Microsoft.AspNetCore.Mvc;
using SportBook.Api.Response;
using SportBook.Core.DTOs;
using SportBook.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SportBook.Api.Controllers
{
    [ApiController]
    [Route("api/disponibilidad")]
    public class DisponibilidadController : ControllerBase
    {
        private readonly IReservaRepository _reservaRepository;
        private readonly ILogger<DisponibilidadController> _logger;

        public DisponibilidadController(
            IReservaRepository reservaRepository,
            ILogger<DisponibilidadController> logger)
        {
            _reservaRepository = reservaRepository;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult> GetDisponibilidad(
            [FromQuery] DateOnly fecha)
        {
            if (fecha < DateOnly.FromDateTime(DateTime.Today))
            {
                return BadRequest(
                    new ApiResponse<string>(
                        "La fecha no puede ser anterior a hoy."));
            }

            try
            {
                var resultado =
                    await _reservaRepository.GetDisponibilidadAsync(fecha);

                return Ok(
                    new ApiResponse<IEnumerable<DisponibilidadDto>>(resultado));
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex, "Error al consultar la disponibilidad.");

                return StatusCode(
                    500,
                    new ApiResponse<string>(
                        "No se pudo consultar la disponibilidad."));
            }
        }
    }
}
