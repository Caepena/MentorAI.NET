using MentorAI.Domain.Entities;
using MentorAI.Domain.Interfaces;
using MonitoringMottu.Domain.Pagination;

namespace MentorAI.Application.UseCases;

public class SkillUseCase : ISkillUseCase
{
    private readonly ISkillRepository _repository;
    
    public SkillUseCase(ISkillRepository repository) => _repository = repository;
    
    public Task<PageResult<Skill>> GetPaginationAsyncSkill(int page, int pageSize) =>
        _repository.GetPaginationAsyncSkill(page, pageSize);
}