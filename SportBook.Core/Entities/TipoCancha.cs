using System;
using System.Collections.Generic;

namespace SportBook.Core.Entities;

public partial class TipoCancha
{
    public long IdTipoCancha { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Descripcion { get; set; }

    public virtual ICollection<Cancha> Canchas { get; set; } = new List<Cancha>();
}
