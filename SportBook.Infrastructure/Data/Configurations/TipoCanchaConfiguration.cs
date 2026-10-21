
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportBook.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportBook.Infrastructure.Data.Configurations
{
    public class TipoCanchaConfiguration :
        IEntityTypeConfiguration<TipoCancha>
    {
        public void Configure(EntityTypeBuilder<TipoCancha> entity)
        {
            entity.HasKey(e => e.IdTipoCancha).HasName("PRIMARY");

            entity.ToTable("tipo_cancha");

            entity.HasIndex(e => e.Nombre, "nombre").IsUnique();

            entity.Property(e => e.IdTipoCancha)
                .HasColumnName("id_tipo_cancha");

            entity.Property(e => e.Descripcion)
                .HasMaxLength(150)
                .HasColumnName("descripcion");

            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .HasColumnName("nombre");
        }
    }
}
