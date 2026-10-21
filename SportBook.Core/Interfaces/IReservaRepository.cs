
using SportBook.Core.DTOs;
using SportBook.Core.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SportBook.Core.Interfaces
{
    public interface IReservaRepository
    {
        Task<IEnumerable<Reserva>> GetAllReservasAsync();

        Task<Reserva?> GetReservaByIdAsync(long id);

        Task InsertReserva(Reserva reserva);

        Task UpdateReserva(Reserva reserva);

        Task<bool> UsuarioActivoExisteAsync(long idUsuario);

        Task<bool> CanchaActivaExisteAsync(long idCancha);

        Task<bool> HorarioActivoExisteAsync(long idHorario);

        Task<bool> EstadoReservaExisteAsync(long idEstado);

        Task<bool> ExisteReservaEnHorarioAsync(
            long idCancha,
            long idHorario,
            DateOnly fecha);
        Task<string> CancelarReservaAsync(long idReserva);

        Task<IEnumerable<DisponibilidadDto>> GetDisponibilidadAsync(DateOnly fecha);

        Task<IEnumerable<Reserva>> GetReservasPorUsuarioAsync(long idUsuario);

        Task<int?> GetCapacidadCanchaAsync(long idCancha);

        Task<string> ActualizarEstadoReservaAsync(
            long idReserva,
            long idEstado);
    }
}
    