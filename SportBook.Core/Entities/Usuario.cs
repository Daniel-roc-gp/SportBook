using System;
using System.Collections.Generic;

namespace SportBook.Core.Entities;

public partial class Usuario
{
    public long IdUsuario { get; set; }

    public long IdRol { get; set; }

    public string Nombre { get; set; } = null!;

    public string Apellido { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string? Telefono { get; set; }

    public bool? Activo { get; set; }

    public DateTime FechaRegistro { get; set; }

    public virtual Rol IdRolNavigation { get; set; } = null!;

    public virtual ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
}
