using MentorAI.Domain.Entities;
using MonitoringMottu.Domain.Pagination;

namespace MentorAI.Domain.Interfaces;

public interface ISkillRepository
{
    Task<PageResult<Skill>>
        GetPaginationAsyncSkill(int page, int pageSize, CancellationToken cancellationToken = default);
}