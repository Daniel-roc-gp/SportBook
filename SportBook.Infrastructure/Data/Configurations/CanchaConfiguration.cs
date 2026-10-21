
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportBook.Core.Entities;

namespace SportBook.Infrastructure.Data.Configurations
{
    public class CanchaConfiguration :
        IEntityTypeConfiguration<Cancha>
    {
        public void Configure(EntityTypeBuilder<Cancha> entity)
        {
            entity.HasKey(e => e.IdCancha).HasName("PRIMARY");

            entity.ToTable("cancha");

            entity.HasIndex(e => e.IdTipoCancha, "fk_cancha_tipo");

            entity.Property(e => e.IdCancha).HasColumnName("id_cancha");
            entity.Property(e => e.Activa)
                .IsRequired()
                .HasDefaultValueSql("'1'")
                .HasColumnName("activa");
            entity.Property(e => e.Capacidad).HasColumnName("capacidad");
            entity.Property(e => e.IdTipoCancha).HasColumnName("id_tipo_cancha");
            entity.Property(e => e.Nombre)
                .HasMaxLength(60)
                .HasColumnName("nombre");
            entity.Property(e => e.PrecioHora)
                .HasPrecision(10, 2)
                .HasColumnName("precio_hora");
            entity.Property(e => e.Techada).HasColumnName("techada");

            entity.HasOne(d => d.IdTipoCanchaNavigation)
                .WithMany(p => p.Canchas)
                .HasForeignKey(d => d.IdTipoCancha)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_cancha_tipo");
        }
    }
}
