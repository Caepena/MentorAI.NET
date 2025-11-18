using MentorAI.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MentorAI.Infrastructure.Mappings
{
    public class CourseMapping : IEntityTypeConfiguration<Course>
    {
        public void Configure(EntityTypeBuilder<Course> builder)
        {

            builder.ToTable("CURSOS");


            builder.HasKey(c => c.Id);

            builder.Property(c => c.Id)
                .ValueGeneratedOnAdd();


            builder.Property(c => c.Titulo)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(c => c.Descricao)
                .HasMaxLength(1000);

            builder.Property(c => c.Provedor)
                .HasMaxLength(100);

            builder.Property(c => c.CargaHoraria)
                .IsRequired();
            
            builder
                .HasMany(c => c.UsuariosMatriculados)
                .WithMany(u => u.CursosAtivos)
                .UsingEntity(j => j.ToTable("USUARIOS_CURSOS"));
        }
    }
}