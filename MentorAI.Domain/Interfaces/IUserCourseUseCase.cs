using MentorAI.Domain.Entities;

namespace MentorAI.Domain.Interfaces;

public interface IUserCourseUseCase
{
    Task MatricularUsuarioEmCursoAsync(
        Guid userId,
        Guid courseId,
        CancellationToken cancellationToken = default
    );
}