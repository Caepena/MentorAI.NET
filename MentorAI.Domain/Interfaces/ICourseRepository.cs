using MentorAI.Domain.Entities;
using MonitoringMottu.Domain.Pagination;

namespace MentorAI.Domain.Interfaces;

public interface ICourseRepository
{
    Task<PageResult<Course>>
        GetPaginationAsyncCourse(int page, int pageSize, CancellationToken cancellationToken = default);
}