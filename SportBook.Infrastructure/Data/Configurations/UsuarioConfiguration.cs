using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportBook.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportBook.Infrastructure.Data.Configurations
{
    public class UsuarioConfiguration :
        IEntityTypeConfiguration<Usuario>
    {
        public void Configure(EntityTypeBuilder<Usuario> entity)
        {
            entity.HasKey(e => e.IdUsuario).HasName("PRIMARY");

            entity.ToTable("usuario");

            entity.HasIndex(e => e.Email, "email").IsUnique();

            entity.HasIndex(e => e.IdRol, "fk_usuario_rol");

            entity.Property(e => e.IdUsuario)
                .HasColumnName("id_usuario");

            entity.Property(e => e.Activo)
                .IsRequired()
                .HasDefaultValueSql("'1'")
                .HasColumnName("activo");

            entity.Property(e => e.Apellido)
                .HasMaxLength(50)
                .HasColumnName("apellido");

            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");

            entity.Property(e => e.FechaRegistro)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("fecha_registro");

            entity.Property(e => e.IdRol)
                .HasColumnName("id_rol");

            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .HasColumnName("nombre");

            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .HasColumnName("password");

            entity.Property(e => e.Telefono)
                .HasMaxLength(20)
                .HasColumnName("telefono");

            entity.HasOne(d => d.IdRolNavigation)
                .WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.IdRol)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_usuario_rol");
        }
    }
}
