
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportBook.Api.Response;
using SportBook.Core.DTOs;
using SportBook.Core.Entities;
using SportBook.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SportBook.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CanchasController : ControllerBase
    {
        private readonly ICanchaRepository _canchaRepository;
        private readonly IMapper _mapper;

        public CanchasController(
            ICanchaRepository canchaRepository,
            IMapper mapper)
        {
            _canchaRepository = canchaRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult> GetCanchas()
        {
            try
            {
                var canchas = await _canchaRepository.GetAllCanchasAsync();
                var canchasDto = _mapper.Map<IEnumerable<CanchaDto>>(canchas);

                return Ok(new ApiResponse<IEnumerable<CanchaDto>>(canchasDto));
            }
            catch (Exception)
            {
                return StatusCode(500,
                    new ApiResponse<string>("Error al consultar las canchas."));
            }
        }

        [HttpGet("{id:long}")]
        public async Task<ActionResult> GetCanchaById(long id)
        {
            try
            {
                var cancha = await _canchaRepository.GetCanchaByIdAsync(id);

                if (cancha == null)
                {
                    return NotFound(
                        new ApiResponse<string>("Cancha no encontrada."));
                }

                var canchaDto = _mapper.Map<CanchaDto>(cancha);

                return Ok(new ApiResponse<CanchaDto>(canchaDto));
            }
            catch (Exception)
            {
                return StatusCode(500,
                    new ApiResponse<string>("Error al consultar la cancha."));
            }
        }

        

        [HttpDelete("{id:long}")]
        public async Task<ActionResult> DeleteCancha(long id)
        {
            try
            {
                var cancha = await _canchaRepository.GetCanchaByIdAsync(id);

                if (cancha == null)
                {
                    return NotFound(
                        new ApiResponse<string>("Cancha no encontrada."));
                }

                await _canchaRepository.DeleteCancha(cancha);

                return Ok(new ApiResponse<string>("Cancha eliminada correctamente."));
            }
            catch (Exception)
            {
                return StatusCode(500,
                    new ApiResponse<string>("Error al eliminar la cancha."));
            }
        }

        [Authorize(Roles = "ADMINISTRADOR")]
        [HttpPost]
        public async Task<ActionResult> InsertCancha(
    [FromBody] CanchaDto canchaDto)
        {
            if (string.IsNullOrWhiteSpace(canchaDto.Nombre) ||
                canchaDto.Nombre.Length > 60 ||
                canchaDto.Capacidad <= 0 ||
                canchaDto.PrecioHora <= 0 ||
                canchaDto.IdTipoCancha <= 0)
            {
                return BadRequest(
                    new ApiResponse<string>("Los datos de la cancha no son válidos."));
            }

            try
            {
                if (!await _canchaRepository.TipoCanchaExisteAsync(
                    canchaDto.IdTipoCancha))
                {
                    return BadRequest(
                        new ApiResponse<string>("El tipo de cancha no existe."));
                }

                var cancha = _mapper.Map<Cancha>(canchaDto);

                cancha.IdCancha = 0;
                cancha.Activa = true;

                await _canchaRepository.InsertCancha(cancha);

                var respuesta = _mapper.Map<CanchaDto>(cancha);

                return CreatedAtAction(
                    nameof(GetCanchaById),
                    new { id = cancha.IdCancha },
                    new ApiResponse<CanchaDto>(respuesta));
            }
            catch (Exception)
            {
                return StatusCode(
                    500,
                    new ApiResponse<string>("Error al registrar la cancha."));
            }
        }

        [Authorize(Roles = "ADMINISTRADOR")]
        [HttpPut("{id:long}")]
        public async Task<ActionResult> UpdateCancha(
    long id,
    [FromBody] CanchaDto canchaDto)
        {
            if (id <= 0 ||
                (canchaDto.IdCancha != 0 && canchaDto.IdCancha != id) ||
                string.IsNullOrWhiteSpace(canchaDto.Nombre) ||
                canchaDto.Nombre.Length > 60 ||
                canchaDto.Capacidad <= 0 ||
                canchaDto.PrecioHora <= 0)
            {
                return BadRequest(
                    new ApiResponse<string>("Los datos de la cancha no son válidos."));
            }

            try
            {
                var cancha = await _canchaRepository.GetCanchaByIdAsync(id);

                if (cancha == null)
                {
                    return NotFound(
                        new ApiResponse<string>("Cancha no encontrada."));
                }

                if (!await _canchaRepository.TipoCanchaExisteAsync(
                    canchaDto.IdTipoCancha))
                {
                    return BadRequest(
                        new ApiResponse<string>("El tipo de cancha no existe."));
                }

                _mapper.Map(canchaDto, cancha);
                cancha.IdCancha = id;

                await _canchaRepository.UpdateCancha(cancha);

                return Ok(new ApiResponse<CanchaDto>(
                    _mapper.Map<CanchaDto>(cancha)));
            }
            catch (Exception)
            {
                return StatusCode(
                    500,
                    new ApiResponse<string>("Error al actualizar la cancha."));
            }
        }

        [Authorize(Roles = "ADMINISTRADOR")]
        [HttpPatch("{id:long}/estado")]
        public async Task<ActionResult> CambiarEstadoCancha(
    long id,
    [FromBody] CambiarEstadoCanchaDto dto)
        {
            if (id <= 0)
            {
                return BadRequest(
                    new ApiResponse<string>("El ID debe ser mayor que cero."));
            }

            try
            {
                var cancha = await _canchaRepository.GetCanchaByIdAsync(id);

                if (cancha == null)
                {
                    return NotFound(
                        new ApiResponse<string>("Cancha no encontrada."));
                }

                cancha.Activa = dto.Activa;

                await _canchaRepository.UpdateCancha(cancha);

                return Ok(new ApiResponse<string>(
                    dto.Activa
                        ? "Cancha activada correctamente."
                        : "Cancha desactivada correctamente."));
            }
            catch (Exception)
            {
                return StatusCode(
                    500,
                    new ApiResponse<string>("Error al cambiar el estado de la cancha."));
            }
        }
    }
}
