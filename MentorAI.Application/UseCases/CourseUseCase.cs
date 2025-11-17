using MentorAI.Domain.Entities;
using MentorAI.Domain.Interfaces;
using MonitoringMottu.Domain.Pagination;

namespace MentorAI.Application.UseCases;

public class CourseUseCase : ICourseUseCase
{
    private readonly ICourseRepository _repository;
    
    public CourseUseCase(ICourseRepository repository) => _repository = repository;
    
    public Task<PageResult<Course>> GetPaginationAsyncCourse(int page, int pageSize) =>
        _repository.GetPaginationAsyncCourse(page, pageSize);
}