using System;
using System.Collections.Generic;

namespace SportBook.Core.Entities;

public partial class Reserva
{
    public long IdReserva { get; set; }

    public string Codigo { get; set; } = null!;

    public long IdUsuario { get; set; }

    public long IdCancha { get; set; }

    public long IdHorario { get; set; }

    public long IdEstado { get; set; }

    public DateOnly Fecha { get; set; }

    public int Jugadores { get; set; }

    public DateTime FechaRegistro { get; set; }

    public virtual ICollection<AuditoriaReserva> AuditoriaReservas { get; set; } = new List<AuditoriaReserva>();

    public virtual Cancha IdCanchaNavigation { get; set; } = null!;

    public virtual EstadoReserva IdEstadoNavigation { get; set; } = null!;

    public virtual Horario IdHorarioNavigation { get; set; } = null!;

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;
}
