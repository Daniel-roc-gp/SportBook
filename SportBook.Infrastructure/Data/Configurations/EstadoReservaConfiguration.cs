
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportBook.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportBook.Infrastructure.Data.Configurations
{
    public class EstadoReservaConfiguration :
        IEntityTypeConfiguration<EstadoReserva>
    {
        public void Configure(EntityTypeBuilder<EstadoReserva> entity)
        {
            entity.HasKey(e => e.IdEstado).HasName("PRIMARY");

            entity.ToTable("estado_reserva");

            entity.HasIndex(e => e.Nombre, "nombre").IsUnique();

            entity.Property(e => e.IdEstado)
                .HasColumnName("id_estado");

            entity.Property(e => e.Nombre)
                .HasMaxLength(20)
                .HasColumnName("nombre");
        }
    }
}
