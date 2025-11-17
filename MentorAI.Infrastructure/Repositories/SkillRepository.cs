using MentorAI.Domain.Entities;
using MentorAI.Domain.Interfaces;
using MentorAI.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using MonitoringMottu.Domain.Pagination;

namespace MentorAI.Infrastructure.Repositories;

public class SkillRepository : ISkillRepository
{
    private readonly MentorAIContext _context;

    public SkillRepository(MentorAIContext context) => _context = context;

    public async Task<PageResult<Skill>> GetPaginationAsyncSkill(
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        if (page <= 0) page = 1;
        if (pageSize <= 0) pageSize = 10;

        var query = _context.Skills
            .AsNoTracking()
            .OrderBy(s => s.Nome);

        var total = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PageResult<Skill>
        {
            Items = items,
            Total = total,
            Page = page,
            PageSize = pageSize
        };
    }
}