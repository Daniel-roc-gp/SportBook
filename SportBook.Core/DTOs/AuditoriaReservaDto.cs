
using System;
using System.Collections.Generic;
using System.Text;


namespace SportBook.Core.DTOs
{
    public class AuditoriaReservaDto
    {
        public long IdAuditoria { get; set; }

        public long IdReserva { get; set; }

        public string Accion { get; set; } = null!;

        public DateTime? FechaCambio { get; set; }

        public string? Detalle { get; set; }
    }
}
