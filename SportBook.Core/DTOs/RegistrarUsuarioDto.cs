namespace SportBook.Core.DTOs
{
    public class RegistrarUsuarioDto
    {
        public string Nombre { get; set; } = null!;

        public string Apellido { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string Password { get; set; } = null!;

        public string? Telefono { get; set; }
    }
}