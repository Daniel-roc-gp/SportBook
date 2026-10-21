using System;
using System.Collections.Generic;
using System.Text;

namespace SportBook.Core.DTOs
{
    public class ReservaDto
    {
        public long IdReserva { get; set; }

        public string Codigo { get; set; } = null!;

        public long IdUsuario { get; set; }

        public long IdCancha { get; set; }

        public long IdHorario { get; set; }

        public long IdEstado { get; set; }

        public DateOnly Fecha { get; set; }

        public int Jugadores { get; set; }

        public DateTime? FechaRegistro { get; set; }
    }
}

