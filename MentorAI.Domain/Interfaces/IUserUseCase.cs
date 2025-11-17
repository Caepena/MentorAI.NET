using MentorAI.Domain.Entities;
using MentorAI.Domain.Pagination;

namespace MentorAI.Domain.Interfaces;

public interface IUserUseCase
{
    Task<PageResult<User>> GetPaginationAsyncUser(int page, int pageSize);
}