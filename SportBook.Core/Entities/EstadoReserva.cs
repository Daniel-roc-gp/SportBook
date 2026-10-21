using System;
using System.Collections.Generic;

namespace SportBook.Core.Entities;

public partial class EstadoReserva
{
    public long IdEstado { get; set; }

    public string Nombre { get; set; } = null!;

    public virtual ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
}
