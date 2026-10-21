
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportBook.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportBook.Infrastructure.Data.Configurations
{
    public class ReservaConfiguration :
        IEntityTypeConfiguration<Reserva>
    {
        public void Configure(EntityTypeBuilder<Reserva> entity)
        {
            entity.HasKey(e => e.IdReserva).HasName("PRIMARY");

            entity.ToTable("reserva");

            entity.HasIndex(e => e.Codigo, "codigo").IsUnique();

            entity.HasIndex(e => e.IdCancha, "fk_reserva_cancha");

            entity.HasIndex(e => e.IdEstado, "fk_reserva_estado");

            entity.HasIndex(e => e.IdHorario, "fk_reserva_horario");

            entity.HasIndex(e => e.IdUsuario, "fk_reserva_usuario");

            entity.HasIndex(
                e => new { e.Fecha, e.IdCancha },
                "idx_reserva_fecha_cancha");

            entity.Property(e => e.IdReserva)
                .HasColumnName("id_reserva");

            entity.Property(e => e.Codigo)
                .HasMaxLength(20)
                .HasColumnName("codigo");

            entity.Property(e => e.Fecha)
                .HasColumnName("fecha");

            entity.Property(e => e.FechaRegistro)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("fecha_registro");

            entity.Property(e => e.IdCancha)
                .HasColumnName("id_cancha");

            entity.Property(e => e.IdEstado)
                .HasColumnName("id_estado");

            entity.Property(e => e.IdHorario)
                .HasColumnName("id_horario");

            entity.Property(e => e.IdUsuario)
                .HasColumnName("id_usuario");

            entity.Property(e => e.Jugadores)
                .HasColumnName("jugadores");

            entity.HasOne(d => d.IdCanchaNavigation)
                .WithMany(p => p.Reservas)
                .HasForeignKey(d => d.IdCancha)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_reserva_cancha");

            entity.HasOne(d => d.IdEstadoNavigation)
                .WithMany(p => p.Reservas)
                .HasForeignKey(d => d.IdEstado)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_reserva_estado");

            entity.HasOne(d => d.IdHorarioNavigation)
                .WithMany(p => p.Reservas)
                .HasForeignKey(d => d.IdHorario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_reserva_horario");

            entity.HasOne(d => d.IdUsuarioNavigation)
                .WithMany(p => p.Reservas)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_reserva_usuario");
        }
    }
}
