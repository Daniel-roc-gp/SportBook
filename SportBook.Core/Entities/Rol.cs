using System;
using System.Collections.Generic;

namespace SportBook.Core.Entities;

public partial class Rol
{
    public long IdRol { get; set; }

    public string Nombre { get; set; } = null!;

    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}
