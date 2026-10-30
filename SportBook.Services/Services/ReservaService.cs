using SportBook.Core.DTOs;
using SportBook.Core.Entities;
using SportBook.Core.Interfaces;
using SportBook.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportBook.Services.Services
{
    public class ReservaService : IReservaService
    {
        public readonly IBaseRepository<Reserva> _baseRepository;
        public readonly IReservaRepository _reservaRepository;

        public ReservaService(
            IBaseRepository<Reserva> baseRepository,
            IReservaRepository reservaRepository)
        {
            _baseRepository = baseRepository;
            _reservaRepository = reservaRepository;
        }

        public async Task<IEnumerable<Reserva>> GetAllReservasAsync()
        {
            return await _baseRepository.GetAll();
        }

        public async Task<Reserva?> GetReservaByIdAsync(long id)
        {
            return await _reservaRepository.GetReservaByIdAsync(id);
        }

        public async Task<string> InsertReserva(Reserva reserva)
        {
            // 1. Verificar que el usuario exista y esté activo
            if (!await _reservaRepository.UsuarioActivoExisteAsync(
                reserva.IdUsuario))
            {
                return "USER_INVALID";
            }

            // 2. Verificar que la cancha exista y esté activa
            if (!await _reservaRepository.CanchaActivaExisteAsync(
                reserva.IdCancha))
            {
                return "COURT_INVALID";
            }

            // 3. Verificar que el horario exista y esté activo
            if (!await _reservaRepository.HorarioActivoExisteAsync(
                reserva.IdHorario))
            {
                return "SCHEDULE_INVALID";
            }

            // 4. Verificar que el estado de reserva exista
            if (!await _reservaRepository.EstadoReservaExisteAsync(
                reserva.IdEstado))
            {
                return "STATE_INVALID";
            }

            // 5. Verificar la capacidad de la cancha
            var capacidad = await _reservaRepository.GetCapacidadCanchaAsync(
                reserva.IdCancha);

            if (capacidad == null)
            {
                return "COURT_NOT_FOUND";
            }

            if (reserva.Jugadores > capacidad.Value)
            {
                return "CAPACITY_EXCEEDED";
            }

            // 6. Verificar que el horario no esté ocupado
            if (await _reservaRepository.ExisteReservaEnHorarioAsync(
                reserva.IdCancha,
                reserva.IdHorario,
                reserva.Fecha))
            {
                return "SLOT_OCCUPIED";
            }

            // 7. Registrar la reserva y su auditoría
            await _reservaRepository.InsertReserva(reserva);

            return "OK";
        }

        public async Task UpdateReserva(Reserva reserva)
        {
            await _reservaRepository.UpdateReserva(reserva);
        }

        public async Task<string> CancelarReservaAsync(long id)
        {
            return await _reservaRepository.CancelarReservaAsync(id);
        }

        public async Task<IEnumerable<Reserva>> GetReservasPorUsuarioAsync(
            long idUsuario)
        {
            return await _reservaRepository.GetReservasPorUsuarioAsync(
                idUsuario);
        }

        public async Task<string> ActualizarEstadoReservaAsync(
            long idReserva,
            long idEstado)
        {
            return await _reservaRepository.ActualizarEstadoReservaAsync(
                idReserva,
                idEstado);
        }

        public async Task<IEnumerable<DisponibilidadDto>> GetDisponibilidadAsync(
            DateOnly fecha)
        {
            return await _reservaRepository.GetDisponibilidadAsync(fecha);
        }
    }
}
