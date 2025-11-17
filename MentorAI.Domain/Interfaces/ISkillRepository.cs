using MentorAI.Domain.Entities;
using MentorAI.Domain.Pagination;

namespace MentorAI.Domain.Interfaces;

public interface ISkillRepository
{
    Task<PageResult<Skill>>
        GetPaginationAsyncSkill(int page, int pageSize, CancellationToken cancellationToken = default);
}