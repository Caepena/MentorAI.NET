using MentorAI.Domain.Entities;
using MentorAI.Domain.Pagination;

namespace MentorAI.Domain.Interfaces;

public interface ICourseRepository
{
    Task<PageResult<Course>>
        GetPaginationAsyncCourse(int page, int pageSize, CancellationToken cancellationToken = default);
}