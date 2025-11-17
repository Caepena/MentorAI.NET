using MentorAI.Domain.Entities;
using MentorAI.Domain.Interfaces;
using MentorAI.Domain.Pagination;
using MentorAI.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace MentorAI.Infrastructure.Repositories;

public class CourseRepository : ICourseRepository
{
    private readonly MentorAIContext _context;

    public CourseRepository(MentorAIContext context) => _context = context;

    public async Task<PageResult<Course>> GetPaginationAsyncCourse(
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        if (page <= 0) page = 1;
        if (pageSize <= 0) pageSize = 10;

        var query = _context.Courses
            .AsNoTracking()
            .Include(c => c.Skill)
            .OrderBy(c => c.Titulo);

        var total = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PageResult<Course>
        {
            Items = items,
            Total = total,
            Page = page,
            PageSize = pageSize
        };
    }
}