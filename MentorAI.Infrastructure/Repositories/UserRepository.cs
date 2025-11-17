using MentorAI.Domain.Entities;
using MentorAI.Domain.Interfaces;
using MentorAI.Domain.Pagination;
using MentorAI.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace MentorAI.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly MentorAIContext _context;

    public UserRepository(MentorAIContext context) => _context = context;

    public async Task<PageResult<User>> GetPaginationAsyncUser(
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        if (page <= 0) page = 1;
        if (pageSize <= 0) pageSize = 10;

        var query = _context.Users
            .AsNoTracking()
            .OrderBy(u => u.Nome);

        var total = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PageResult<User>
        {
            Items = items,
            Total = total,
            Page = page,
            PageSize = pageSize
        };
    }
}