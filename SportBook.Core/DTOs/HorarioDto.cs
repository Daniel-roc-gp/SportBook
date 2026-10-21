using System;
using System.Collections.Generic;
using System.Text;


namespace SportBook.Core.DTOs
{
    public class HorarioDto
    {
        public long IdHorario { get; set; }

        public TimeSpan HoraInicio { get; set; }

        public TimeSpan HoraFin { get; set; }

        public bool? Activo { get; set; }
    }
}

