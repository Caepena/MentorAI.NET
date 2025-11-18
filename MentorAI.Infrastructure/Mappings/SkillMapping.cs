using MentorAI.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MentorAI.Infrastructure.Mappings
{
    public class SkillMapping : IEntityTypeConfiguration<Skill>
    {
        public void Configure(EntityTypeBuilder<Skill> builder)
        {
            builder.ToTable("SKILLS");

            builder.HasKey(s => s.Id);

            builder.Property(s => s.Id)
                .ValueGeneratedOnAdd();

            builder.Property(s => s.Nome)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(s => s.Descricao)
                .HasMaxLength(500);
            
        }
    }
}