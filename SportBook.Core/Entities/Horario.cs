using System;
using System.Collections.Generic;

namespace SportBook.Core.Entities;

public partial class Horario
{
    public long IdHorario { get; set; }

    public TimeOnly HoraInicio { get; set; }

    public TimeOnly HoraFin { get; set; }

    public bool? Activo { get; set; }

    public virtual ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
}
