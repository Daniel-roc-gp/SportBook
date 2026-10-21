
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using SportBook.Core.Entities;

namespace SportBook.Infrastructure.Data
{
    public partial class SportBookDbContext : DbContext
    {
        public SportBookDbContext(
            DbContextOptions<SportBookDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AuditoriaReserva> AuditoriaReservas { get; set; }

        public virtual DbSet<Cancha> Canchas { get; set; }

        public virtual DbSet<EstadoReserva> EstadoReservas { get; set; }

        public virtual DbSet<Horario> Horarios { get; set; }

        public virtual DbSet<Reserva> Reservas { get; set; }

        public virtual DbSet<Rol> Rols { get; set; }

        public virtual DbSet<TipoCancha> TipoCanchas { get; set; }

        public virtual DbSet<Usuario> Usuarios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .UseCollation("utf8mb4_0900_ai_ci")
                .HasCharSet("utf8mb4");

            modelBuilder.ApplyConfigurationsFromAssembly(
                Assembly.GetExecutingAssembly());

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
