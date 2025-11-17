using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace MentorAI.Infrastructure.Context;

public class MentorAIContextFactory : IDesignTimeDbContextFactory<MentorAIContext>
{
    public MentorAIContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<MentorAIContext>();

        optionsBuilder.UseOracle("User ID=rm557984;Password=191101;Data Source=oracle.fiap.com.br:1521/orcl;");
        
        return new MentorAIContext(optionsBuilder.Options);
    }
    
}