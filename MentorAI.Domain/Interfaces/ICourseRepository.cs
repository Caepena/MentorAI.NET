using MentorAI.Domain.Entities;
using MentorAI.Domain.Pagination;

namespace MentorAI.Domain.Interfaces;

public interface ICourseRepository
{
    Task<PageResult<Course>>
        GetPaginationAsyncCourse(int page, int pageSize, CancellationToken cancellationToken = default);
    
    Task<List<Course>> GetAllWithRelationsAsync(
        CancellationToken cancellationToken = default
    );

    Task<Course?> GetByIdWithRelationsAsync(
        Guid id,
        CancellationToken cancellationToken = default
    );
}