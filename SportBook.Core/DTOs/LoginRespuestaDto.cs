namespace SportBook.Core.DTOs
{
    public class LoginRespuestaDto
    {
        public string Token { get; set; } = null!;

        public long IdUsuario { get; set; }

        public string Nombre { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string Rol { get; set; } = null!;
    }
}