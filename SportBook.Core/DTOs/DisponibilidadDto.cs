
namespace SportBook.Core.DTOs
{
    public class DisponibilidadDto
    {
        public long IdCancha { get; set; }

        public string NombreCancha { get; set; } = null!;

        public string TipoCancha { get; set; } = null!;

        public int Capacidad { get; set; }

        public bool Techada { get; set; }

        public decimal PrecioHora { get; set; }

        public long IdHorario { get; set; }

        public string HoraInicio { get; set; } = null!;

        public string HoraFin { get; set; } = null!;
    }
}
