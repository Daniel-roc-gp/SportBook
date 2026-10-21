using System;
using System.Collections.Generic;
using System.Text;


namespace SportBook.Core.DTOs
{
    public class TipoCanchaDto
    {
        public long IdTipoCancha { get; set; }

        public string Nombre { get; set; } = null!;

        public string? Descripcion { get; set; }
    }
}
