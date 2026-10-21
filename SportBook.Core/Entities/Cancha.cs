using System;
using System.Collections.Generic;

namespace SportBook.Core.Entities;

public partial class Cancha
{
    public long IdCancha { get; set; }

    public long IdTipoCancha { get; set; }

    public string Nombre { get; set; } = null!;

    public int Capacidad { get; set; }

    public bool Techada { get; set; }

    public bool? Activa { get; set; }

    public decimal PrecioHora { get; set; }

    public virtual TipoCancha IdTipoCanchaNavigation { get; set; } = null!;

    public virtual ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
}
