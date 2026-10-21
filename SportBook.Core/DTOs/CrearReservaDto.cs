using System;

namespace SportBook.Core.DTOs
{
    public class CrearReservaDto
    {
        public long IdUsuario { get; set; }

        public long IdCancha { get; set; }

        public long IdHorario { get; set; }

        public long IdEstado { get; set; }

        public DateOnly Fecha { get; set; }

        public int Jugadores { get; set; }
    }
}