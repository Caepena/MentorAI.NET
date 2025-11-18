using System.Reflection;
using System.Text.Json.Serialization;
using MentorAI.Application.UseCases;
using MentorAI.Domain.Interfaces;
using MentorAI.Extensions;
using MentorAI.Infrastructure.Context;
using MentorAI.Infrastructure.Repositories;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

namespace MentorAI.API;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Configuration
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables();

        builder.Services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            });

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(x =>
        {
            x.SwaggerDoc("v1", new OpenApiInfo()
            {
                Title = builder.Configuration["Swagger:Title"] ?? "MentorAI API",
                Description = "API para recomendação de trilhas de aprendizado, requalificação e produtividade profissional no projeto Global Solution.",
                Contact = new OpenApiContact
                {
                    Name = "Equipe MentorAI",
                    Email = "mentoriai@fiap.com.br"
                }
            });

            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
                x.IncludeXmlComments(xmlPath);
        });

        builder.Services.AddDbContext<MentorAIContext>(options =>
            options.UseOracle(builder.Configuration.GetConnectionString("OracleConnection")));

        builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<ICourseRepository, CourseRepository>();
        builder.Services.AddScoped<ISkillRepository, SkillRepository>();
        builder.Services.AddScoped<IUserCourseUseCase, UserCourseUseCase>();


        builder.Services.AddChecks(builder.Configuration);

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseAuthorization();

        app.MapControllers();

        app.MapHealthChecks("/health-check", new HealthCheckOptions
        {
            ResponseWriter = HealthCheckExtensions.WriteResponse
        });

        app.Run();
    }
}