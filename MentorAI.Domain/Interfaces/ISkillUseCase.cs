using MentorAI.Domain.Entities;
using MentorAI.Domain.Pagination;

namespace MentorAI.Domain.Interfaces;

public interface ISkillUseCase
{
    Task<PageResult<Skill>> GetPaginationAsyncSkill(int page, int pageSize);
}