using MentorAI.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MentorAI.Infrastructure.Mappings
{
    public class UserMapping : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("USUARIOS");
            builder.HasKey(u => u.Id);


            builder.Property(u => u.Id)
                .HasColumnName("ID")
                .IsRequired()
                .ValueGeneratedOnAdd();

            builder.Property(u => u.Nome)
                .HasColumnName("NOME")
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(u => u.Email)
                .HasColumnName("EMAIL")
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(u => u.CargoAtual)
                .HasColumnName("CARGO_ATUAL")
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(u => u.CargoDesejado)
                .HasColumnName("CARGO_DESEJADO")
                .IsRequired()
                .HasMaxLength(100);
            
            builder
                .HasMany(u => u.CursosAtivos)
                .WithMany(c => c.UsuariosMatriculados)
                .UsingEntity(j =>
                        j.ToTable("USUARIOS_CURSOS")
                );
        }
    }
}