using System;
using System.Collections.Generic;
using System.Text;


namespace SportBook.Core.DTOs
{
    public class UsuarioDto
    {
        public long IdUsuario { get; set; }

        public long IdRol { get; set; }

        public string Nombre { get; set; } = null!;

        public string Apellido { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string? Telefono { get; set; }

        public bool? Activo { get; set; }

        public DateTime? FechaRegistro { get; set; }
    }
}

