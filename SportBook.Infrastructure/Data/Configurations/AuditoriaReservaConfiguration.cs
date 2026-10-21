
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportBook.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportBook.Infrastructure.Data.Configurations
{
    public class AuditoriaReservaConfiguration :
        IEntityTypeConfiguration<AuditoriaReserva>
    {
        public void Configure(EntityTypeBuilder<AuditoriaReserva> entity)
        {
            entity.HasKey(e => e.IdAuditoria).HasName("PRIMARY");

            entity.ToTable("auditoria_reserva");

            entity.HasIndex(e => e.IdReserva, "fk_auditoria_reserva");

            entity.Property(e => e.IdAuditoria)
                .HasColumnName("id_auditoria");

            entity.Property(e => e.Accion)
                .HasMaxLength(100)
                .HasColumnName("accion");

            entity.Property(e => e.Detalle)
                .HasMaxLength(255)
                .HasColumnName("detalle");

            entity.Property(e => e.FechaCambio)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("fecha_cambio");

            entity.Property(e => e.IdReserva)
                .HasColumnName("id_reserva");

            entity.HasOne(d => d.IdReservaNavigation)
                .WithMany(p => p.AuditoriaReservas)
                .HasForeignKey(d => d.IdReserva)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_auditoria_reserva");
        }
    }
}
