
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportBook.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportBook.Infrastructure.Data.Configurations
{
    public class HorarioConfiguration :
        IEntityTypeConfiguration<Horario>
    {
        public void Configure(EntityTypeBuilder<Horario> entity)
        {
            entity.HasKey(e => e.IdHorario).HasName("PRIMARY");

            entity.ToTable("horario");

            entity.HasIndex(
                e => new { e.HoraInicio, e.HoraFin },
                "hora_inicio").IsUnique();

            entity.Property(e => e.IdHorario)
                .HasColumnName("id_horario");

            entity.Property(e => e.Activo)
                .IsRequired()
                .HasDefaultValueSql("'1'")
                .HasColumnName("activo");

            entity.Property(e => e.HoraFin)
                .HasColumnType("time")
                .HasColumnName("hora_fin");

            entity.Property(e => e.HoraInicio)
                .HasColumnType("time")
                .HasColumnName("hora_inicio");
        }
    }
}
