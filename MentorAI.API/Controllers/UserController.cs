using System.Net;
using MentorAI.API.Models;
using MentorAI.Domain.Entities;
using MentorAI.Domain.Interfaces;
using MentorAI.Domain.Pagination;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace MentorAI.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly LinkGenerator _links;
        private readonly IUserRepository _usersRepository;
        private readonly IRepository<User> _userRepository;
        private readonly IUserCourseUseCase _userCourseUseCase;

        public UserController(
            IRepository<User> userRepository,
            LinkGenerator links,
            IUserRepository usersRepository,
            IUserCourseUseCase userCourseUseCase)
        {
            _userRepository = userRepository;
            _usersRepository = usersRepository;
            _links = links ?? throw new ArgumentException(nameof(links));
            _userCourseUseCase = userCourseUseCase;
        }

        // GET: /User
        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _userRepository.GetAllAsync();
            return Ok(users);
        }

        // GET: /User/{id}
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetUser(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                return NotFound("Usuário não existe ou não foi encontrado");

            return Ok(user);
        }

        // PUT: /User/{id}
        [HttpPut("{id:guid}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> PutUser(Guid id, [FromBody] UserInputModel inputModel)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userExiste = await _userRepository.GetByIdAsync(id);
            if (userExiste == null)
                return NotFound("Usuário não existe ou não foi encontrado");

            try
            {
                userExiste.Refresh(
                    nome: inputModel.Nome,
                    email: inputModel.Email,
                    cargoAtual: inputModel.CargoAtual,
                    cargoDesejado: inputModel.CargoDesejado
                );

                await _userRepository.UpdateAsync(userExiste);
                await _userRepository.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Erro ao atualizar usuário: {e.Message}");
            }
        }

        // POST: /User
        [HttpPost]
        [ProducesResponseType(typeof(User), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> PostUser([FromBody] UserInputModel inputModel)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var user = new User(
                    nome: inputModel.Nome,
                    email: inputModel.Email,
                    cargoAtual: inputModel.CargoAtual,
                    cargoDesejado: inputModel.CargoDesejado
                );

                await _userRepository.AddAsync(user);
                await _userRepository.SaveChangesAsync();

                return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
            }
            catch (Exception e)
            {
                return BadRequest($"Erro ao cadastrar usuário: {e.Message}");
            }
        }

        // DELETE: /User/{id}
        [HttpDelete("{id:guid}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                return NotFound("Usuário não existe ou não foi encontrado");

            try
            {
                await _userRepository.DeleteAsync(id);
                await _userRepository.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception e)
            {
                return BadRequest($"Erro ao deletar o usuário: {e.Message}");
            }
        }

        // GET: /User/paginado
        [HttpGet("paginado", Name = "GetUsersPaged")]
        [Produces("application/hal+json")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPaged(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null,
            [FromQuery] string? sortDir = "Asc",
            CancellationToken ct = default)
        {
            var pr = await _usersRepository.GetPaginationAsyncUser(page, pageSize, ct);
            pr ??= new PageResult<User> { Items = Array.Empty<User>(), Page = page, PageSize = pageSize, Total = 0 };

            var items = pr.Items.Select(u => new User.UserResponse(
                u.Id,
                u.Nome,
                u.Email,
                u.CargoAtual,
                u.CargoDesejado
            )).ToList();

            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 10;

            var total = pr.Total;
            var totalPages = (int)Math.Ceiling(total / (double)pageSize);
            if (totalPages == 0) totalPages = 1;

            var selfPage = Math.Clamp(page, 1, totalPages);

            string? LinkTo(int targetPage)
            {
                return _links.GetUriByName(
                    HttpContext,
                    "GetUsersPaged",
                    values: new
                    {
                        page = targetPage,
                        pageSize,
                        search,
                        sortDir
                    });
            }

            var linkSelf  = LinkTo(selfPage);
            var linkFirst = LinkTo(1);
            var linkLast  = LinkTo(totalPages);
            var linkPrev  = selfPage > 1          ? LinkTo(selfPage - 1) : null;
            var linkNext  = selfPage < totalPages ? LinkTo(selfPage + 1) : null;

            var links = new Dictionary<string, object>();
            if (linkSelf  is not null) links["self"]  = new { href = linkSelf  };
            if (linkFirst is not null) links["first"] = new { href = linkFirst };
            if (linkPrev  is not null) links["prev"]  = new { href = linkPrev  };
            if (linkNext  is not null) links["next"]  = new { href = linkNext  };
            if (linkLast  is not null) links["last"]  = new { href = linkLast  };

            var body = new
            {
                _embedded = new { usuarios = items },
                _links = links,
                page = new
                {
                    size = pageSize,
                    totalElements = total,
                    totalPages,
                    number = selfPage - 1 // zero-based
                }
            };

            return Ok(body);
        }
        
        // POST: /User/{userId}/courses/{courseId}
        [HttpPost("{userId:guid}/courses/{courseId:guid}")]
        public async Task<IActionResult> MatricularEmCurso(
            Guid userId,
            Guid courseId,
            CancellationToken ct)
        {
            try
            {
                await _userCourseUseCase.MatricularUsuarioEmCursoAsync(userId, courseId, ct);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Erro ao matricular usuário no curso: {e.Message}");
            }
        }
    }
}
