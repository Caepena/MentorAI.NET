using MentorAI.Domain.Entities;
using MentorAI.Domain.Interfaces;
using MentorAI.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace MentorAI.Application.UseCases;

public class UserCourseUseCase : IUserCourseUseCase
{
    private readonly MentorAIContext _context;

    public UserCourseUseCase(MentorAIContext context)
    {
        _context = context;
    }

    public async Task MatricularUsuarioEmCursoAsync(
        Guid userId,
        Guid courseId,
        CancellationToken cancellationToken = default)
    {
        var user = await _context.Users
            .Include(u => u.CursosAtivos)
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (user is null)
            throw new InvalidOperationException("Usuário não encontrado.");

        var course = await _context.Courses
            .FirstOrDefaultAsync(c => c.Id == courseId, cancellationToken);

        if (course is null)
            throw new InvalidOperationException("Curso não encontrado.");

        var jaMatriculado = user.CursosAtivos.Any(c => c.Id == courseId);
        if (jaMatriculado)
            return; // ou lançar uma exceção se preferir

        user.CursosAtivos.Add(course);

        await _context.SaveChangesAsync(cancellationToken);
    }
}