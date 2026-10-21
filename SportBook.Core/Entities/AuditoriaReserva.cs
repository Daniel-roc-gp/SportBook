using System;
using System.Collections.Generic;

namespace SportBook.Core.Entities;

public partial class AuditoriaReserva
{
    public long IdAuditoria { get; set; }

    public long IdReserva { get; set; }

    public string Accion { get; set; } = null!;

    public DateTime FechaCambio { get; set; }

    public string? Detalle { get; set; }

    public virtual Reserva IdReservaNavigation { get; set; } = null!;
}
