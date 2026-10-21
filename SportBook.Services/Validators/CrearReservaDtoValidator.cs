using FluentValidation;
using SportBook.Core.DTOs;
using System;

namespace SportBook.Services.Validators
{
    public class CrearReservaDtoValidator :
        AbstractValidator<CrearReservaDto>
    {
        public CrearReservaDtoValidator()
        {
            RuleFor(x => x.IdUsuario)
                .GreaterThan(0)
                .WithMessage("El ID del usuario debe ser mayor que cero.");

            RuleFor(x => x.IdCancha)
                .GreaterThan(0)
                .WithMessage("El ID de la cancha debe ser mayor que cero.");

            RuleFor(x => x.IdHorario)
                .GreaterThan(0)
                .WithMessage("El ID del horario debe ser mayor que cero.");

            RuleFor(x => x.IdEstado)
                .GreaterThan(0)
                .WithMessage("El estado de la reserva es obligatorio.");

            RuleFor(x => x.Fecha)
                .Must(fecha =>
                    fecha >= DateOnly.FromDateTime(DateTime.Today))
                .WithMessage("La fecha no puede ser anterior a hoy.");

            RuleFor(x => x.Jugadores)
                .GreaterThan(0)
                .WithMessage("Debe haber al menos un jugador.");
        }
    }
}