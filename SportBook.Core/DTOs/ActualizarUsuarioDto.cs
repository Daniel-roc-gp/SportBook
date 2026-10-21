namespace SportBook.Core.DTOs
{
    public class ActualizarUsuarioDto
    {
        public string Nombre { get; set; } = null!;

        public string Apellido { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string? Telefono { get; set; }

        public bool Activo { get; set; }
    }
}