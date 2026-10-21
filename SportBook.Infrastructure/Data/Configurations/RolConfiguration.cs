
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportBook.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportBook.Infrastructure.Data.Configurations
{
    public class RolConfiguration :
        IEntityTypeConfiguration<Rol>
    {
        public void Configure(EntityTypeBuilder<Rol> entity)
        {
            entity.HasKey(e => e.IdRol).HasName("PRIMARY");

            entity.ToTable("rol");

            entity.HasIndex(e => e.Nombre, "nombre").IsUnique();

            entity.Property(e => e.IdRol)
                .HasColumnName("id_rol");

            entity.Property(e => e.Nombre)
                .HasMaxLength(30)
                .HasColumnName("nombre");
        }
    }
}
