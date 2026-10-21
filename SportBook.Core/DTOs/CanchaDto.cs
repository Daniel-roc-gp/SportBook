using System;
using System.Collections.Generic;
using System.Text;


namespace SportBook.Core.DTOs
{
    public class CanchaDto
    {
        public long IdCancha { get; set; }

        public long IdTipoCancha { get; set; }

        public string Nombre { get; set; } = null!;

        public int Capacidad { get; set; }

        public bool Techada { get; set; }

        public bool? Activa { get; set; }

        public decimal PrecioHora { get; set; }
    }
}

