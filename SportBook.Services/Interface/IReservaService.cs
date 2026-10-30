
using SportBook.Core.DTOs;
using SportBook.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportBook.Services.Interfaces
{
    public interface IReservaService
    {
        Task<IEnumerable<Reserva>> GetAllReservasAsync();
        Task<Reserva?> GetReservaByIdAsync(long id);
        Task<string> InsertReserva(Reserva reserva);
        Task UpdateReserva(Reserva reserva);
        Task<string> CancelarReservaAsync(long id);
        Task<IEnumerable<Reserva>> GetReservasPorUsuarioAsync(long idUsuario);
        Task<string> ActualizarEstadoReservaAsync(long idReserva, long idEstado);
        Task<IEnumerable<DisponibilidadDto>> GetDisponibilidadAsync(DateOnly fecha);
    }
}
