using MentorAI.Domain.Entities;
using MentorAI.Domain.Interfaces;
using MentorAI.Domain.Pagination;

namespace MentorAI.Application.UseCases;

public class UserUseCase : IUserUseCase
{
    private readonly IUserRepository _repository;
    
    public UserUseCase(IUserRepository repository) => _repository = repository;
    
    public Task<PageResult<User>> GetPaginationAsyncUser(int page, int pageSize) =>
        _repository.GetPaginationAsyncUser(page, pageSize);
}