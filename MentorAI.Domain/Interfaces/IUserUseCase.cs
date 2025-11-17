using MentorAI.Domain.Entities;
using MonitoringMottu.Domain.Pagination;

namespace MentorAI.Domain.Interfaces;

public interface IUserUseCase
{
    Task<PageResult<User>> GetPaginationAsyncUser(int page, int pageSize);
}