using MentorAI.Domain.Entities;
using MentorAI.Domain.Pagination;

namespace MentorAI.Domain.Interfaces;

public interface IUserRepository
{
    Task<PageResult<User>>
        GetPaginationAsyncUser(int page, int pageSize, CancellationToken cancellationToken = default);
}