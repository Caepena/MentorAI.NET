using MentorAI.Domain.Entities;
using MentorAI.Domain.Pagination;

namespace MentorAI.Domain.Interfaces;

public interface ICourseUseCase
{
    Task<PageResult<Course>> GetPaginationAsyncCourse(int page, int pageSize);
}