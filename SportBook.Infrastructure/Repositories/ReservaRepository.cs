
using Microsoft.EntityFrameworkCore;
using SportBook.Core.DTOs;
using SportBook.Core.Entities;
using SportBook.Core.Interfaces;
using SportBook.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SportBook.Infrastructure.Repositories
{
    public class ReservaRepository : IReservaRepository
    {
        private readonly SportBookDbContext _sportBookContext;

        public ReservaRepository(SportBookDbContext sportBookContext)
        {
            _sportBookContext = sportBookContext;
        }

        public async Task<IEnumerable<Reserva>> GetAllReservasAsync()
        {
            return await _sportBookContext.Reservas
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Reserva?> GetReservaByIdAsync(long id)
        {
            return await _sportBookContext.Reservas
                .Include(r => r.IdEstadoNavigation)
                .FirstOrDefaultAsync(r => r.IdReserva == id);
        }

        public async Task InsertReserva(Reserva reserva)
        {
            await using var transaction =
                await _sportBookContext.Database.BeginTransactionAsync();

            try
            {
                _sportBookContext.Reservas.Add(reserva);
                await _sportBookContext.SaveChangesAsync();

                var auditoria = new AuditoriaReserva
                {
                    IdReserva = reserva.IdReserva,
                    Accion = "CREACION_RESERVA",
                    Detalle = "Reserva creada correctamente."
                };

                _sportBookContext.AuditoriaReservas.Add(auditoria);
                await _sportBookContext.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task UpdateReserva(Reserva reserva)
        {
            await using var transaction =
                await _sportBookContext.Database.BeginTransactionAsync();

            try
            {
                _sportBookContext.Reservas.Update(reserva);
                await _sportBookContext.SaveChangesAsync();

                var auditoria = new AuditoriaReserva
                {
                    IdReserva = reserva.IdReserva,
                    Accion = "ACTUALIZACION_RESERVA",
                    Detalle = "Reserva actualizada correctamente."
                };

                _sportBookContext.AuditoriaReservas.Add(auditoria);
                await _sportBookContext.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> UsuarioActivoExisteAsync(long idUsuario)
        {
            return await _sportBookContext.Usuarios
                .AnyAsync(x =>
                    x.IdUsuario == idUsuario &&
                    x.Activo == true);
        }

        public async Task<bool> CanchaActivaExisteAsync(long idCancha)
        {
            return await _sportBookContext.Canchas
                .AnyAsync(x =>
                    x.IdCancha == idCancha &&
                    x.Activa == true);
        }

        public async Task<bool> HorarioActivoExisteAsync(long idHorario)
        {
            return await _sportBookContext.Horarios
                .AnyAsync(x =>
                    x.IdHorario == idHorario &&
                    x.Activo == true);
        }

        public async Task<bool> EstadoReservaExisteAsync(long idEstado)
        {
            return await _sportBookContext.EstadoReservas
                .AnyAsync(x => x.IdEstado == idEstado);
        }

        public async Task<bool> ExisteReservaEnHorarioAsync(
            long idCancha,
            long idHorario,
            DateOnly fecha)
        {
            return await _sportBookContext.Reservas
                .AnyAsync(x =>
                    x.IdCancha == idCancha &&
                    x.IdHorario == idHorario &&
                    x.Fecha == fecha &&
                    !x.IdEstadoNavigation.Nombre
                        .ToLower()
                        .Contains("cancel"));
        }

        public async Task<int?> GetCapacidadCanchaAsync(long idCancha)
        {
            return await _sportBookContext.Canchas
                .Where(c => c.IdCancha == idCancha)
                .Select(c => (int?)c.Capacidad)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<DisponibilidadDto>> GetDisponibilidadAsync(
            DateOnly fecha)
        {
            var canchas = await _sportBookContext.Canchas
                .AsNoTracking()
                .Include(c => c.IdTipoCanchaNavigation)
                .Where(c => c.Activa == true)
                .ToListAsync();

            var horarios = await _sportBookContext.Horarios
                .AsNoTracking()
                .Where(h => h.Activo == true)
                .ToListAsync();

            var reservas = await _sportBookContext.Reservas
                .AsNoTracking()
                .Include(r => r.IdEstadoNavigation)
                .Where(r => r.Fecha == fecha)
                .ToListAsync();

            var disponibilidad = new List<DisponibilidadDto>();

            foreach (var cancha in canchas)
            {
                foreach (var horario in horarios)
                {
                    var ocupado = reservas.Any(r =>
                        r.IdCancha == cancha.IdCancha &&
                        r.IdHorario == horario.IdHorario &&
                        !r.IdEstadoNavigation.Nombre.Contains(
                            "CANCEL",
                            StringComparison.OrdinalIgnoreCase));

                    if (!ocupado)
                    {
                        disponibilidad.Add(new DisponibilidadDto
                        {
                            IdCancha = cancha.IdCancha,
                            NombreCancha = cancha.Nombre,
                            TipoCancha = cancha.IdTipoCanchaNavigation.Nombre,
                            Capacidad = cancha.Capacidad,
                            Techada = cancha.Techada,
                            PrecioHora = cancha.PrecioHora,
                            IdHorario = horario.IdHorario,
                            HoraInicio = horario.HoraInicio.ToString(),
                            HoraFin = horario.HoraFin.ToString()
                        });
                    }
                }
            }

            return disponibilidad;
        }

        public async Task<IEnumerable<Reserva>> GetReservasPorUsuarioAsync(
            long idUsuario)
        {
            return await _sportBookContext.Reservas
                .AsNoTracking()
                .Where(r => r.IdUsuario == idUsuario)
                .ToListAsync();
        }

        public async Task<string> CancelarReservaAsync(long idReserva)
        {
            await using var transaction =
                await _sportBookContext.Database.BeginTransactionAsync();

            try
            {
                var reserva = await _sportBookContext.Reservas
                    .FirstOrDefaultAsync(r => r.IdReserva == idReserva);

                if (reserva == null)
                {
                    await transaction.RollbackAsync();
                    return "NOT_FOUND";
                }

                var estadoActual = await _sportBookContext.EstadoReservas
                    .FirstOrDefaultAsync(e => e.IdEstado == reserva.IdEstado);

                if (estadoActual == null)
                {
                    await transaction.RollbackAsync();
                    return "STATE_NOT_FOUND";
                }

                var nombreEstadoActual =
                    estadoActual.Nombre.ToUpperInvariant();

                if (nombreEstadoActual == "CANCELADA")
                {
                    await transaction.RollbackAsync();
                    return "ALREADY_CANCELLED";
                }

                if (nombreEstadoActual == "COMPLETADA")
                {
                    await transaction.RollbackAsync();
                    return "CANNOT_CANCEL_COMPLETED";
                }

                var estadoCancelado = await _sportBookContext.EstadoReservas
                    .FirstOrDefaultAsync(e =>
                        e.Nombre.ToUpper() == "CANCELADA");

                if (estadoCancelado == null)
                {
                    await transaction.RollbackAsync();
                    return "CANCEL_STATE_NOT_FOUND";
                }

                reserva.IdEstado = estadoCancelado.IdEstado;

                _sportBookContext.AuditoriaReservas.Add(
                    new AuditoriaReserva
                    {
                        IdReserva = reserva.IdReserva,
                        Accion = "CANCELACION_RESERVA",
                        Detalle = "La reserva fue cancelada."
                    });

                await _sportBookContext.SaveChangesAsync();
                await transaction.CommitAsync();

                return "OK";
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<string> ActualizarEstadoReservaAsync(
            long idReserva,
            long idEstado)
        {
            await using var transaction =
                await _sportBookContext.Database.BeginTransactionAsync();

            try
            {
                var reserva = await _sportBookContext.Reservas
                    .FirstOrDefaultAsync(r => r.IdReserva == idReserva);

                if (reserva == null)
                {
                    await transaction.RollbackAsync();
                    return "NOT_FOUND";
                }

                var estadoActual = await _sportBookContext.EstadoReservas
                    .FirstOrDefaultAsync(e => e.IdEstado == reserva.IdEstado);

                var nuevoEstado = await _sportBookContext.EstadoReservas
                    .FirstOrDefaultAsync(e => e.IdEstado == idEstado);

                if (nuevoEstado == null)
                {
                    await transaction.RollbackAsync();
                    return "STATE_NOT_FOUND";
                }

                if (estadoActual == null)
                {
                    await transaction.RollbackAsync();
                    return "STATE_NOT_FOUND";
                }

                var actual = estadoActual.Nombre.ToUpperInvariant();
                var nuevo = nuevoEstado.Nombre.ToUpperInvariant();

                if (actual == nuevo)
                {
                    await transaction.RollbackAsync();
                    return "ALREADY_IN_STATE";
                }

                var permitido =
                    (actual == "PENDIENTE" &&
                        (nuevo == "CONFIRMADA" || nuevo == "CANCELADA")) ||
                    (actual == "CONFIRMADA" &&
                        (nuevo == "CANCELADA" || nuevo == "COMPLETADA"));

                if (!permitido)
                {
                    await transaction.RollbackAsync();
                    return "INVALID_TRANSITION";
                }

                reserva.IdEstado = nuevoEstado.IdEstado;

                _sportBookContext.AuditoriaReservas.Add(
                    new AuditoriaReserva
                    {
                        IdReserva = reserva.IdReserva,
                        Accion = "CAMBIO_ESTADO_RESERVA",
                        Detalle = $"Cambio de estado: {actual} a {nuevo}."
                    });

                await _sportBookContext.SaveChangesAsync();
                await transaction.CommitAsync();

                return "OK";
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
