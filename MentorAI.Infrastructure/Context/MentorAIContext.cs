using MentorAI.Domain.Entities;
using MentorAI.Infrastructure.Mappings;
using Microsoft.EntityFrameworkCore;

namespace MentorAI.Infrastructure.Context;

public class MentorAIContext : DbContext
{
    public MentorAIContext(DbContextOptions<MentorAIContext> options) : base(options)
    {
    }
    
    public DbSet<User> Users { get; set; }
    public DbSet<Course> Courses { get; set; }
    public DbSet<Skill> Skills { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserMapping());
        modelBuilder.ApplyConfiguration(new CourseMapping());
        modelBuilder.ApplyConfiguration(new SkillMapping());
        base.OnModelCreating(modelBuilder);
    }
}