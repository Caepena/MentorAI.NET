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
    public class SkillController : ControllerBase
    {
        private readonly LinkGenerator _links;
        private readonly ISkillRepository _skillsRepository;
        private readonly IRepository<Skill> _skillRepository;

        public SkillController(
            IRepository<Skill> skillRepository,
            LinkGenerator links,
            ISkillRepository skillsRepository)
        {
            _skillRepository = skillRepository;
            _skillsRepository = skillsRepository;
            _links = links ?? throw new ArgumentException(nameof(links));
        }

        // GET: /Skill
        [HttpGet]
        public async Task<IActionResult> GetSkills()
        {
            var skills = await _skillRepository.GetAllAsync();
            return Ok(skills);
        }

        // GET: /Skill/{id}
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetSkill(Guid id)
        {
            var skill = await _skillRepository.GetByIdAsync(id);
            if (skill == null)
                return NotFound("Skill não existe ou não foi encontrada");

            return Ok(skill);
        }

        // PUT: /Skill/{id}
        [HttpPut("{id:guid}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> PutSkill(Guid id, [FromBody] SkillInputModel inputModel)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var skillExiste = await _skillRepository.GetByIdAsync(id);
            if (skillExiste == null)
                return NotFound("Skill não existe ou não foi encontrada");

            try
            {
                skillExiste.Refresh(
                    nome: inputModel.Nome,
                    descricao: inputModel.Descricao
                );

                await _skillRepository.UpdateAsync(skillExiste);
                await _skillRepository.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Erro ao atualizar a skill: {e.Message}");
            }
        }

        // POST: /Skill
        [HttpPost]
        [ProducesResponseType(typeof(Skill), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> PostSkill([FromBody] SkillInputModel inputModel)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var skill = new Skill(
                    nome: inputModel.Nome,
                    descricao: inputModel.Descricao
                );

                await _skillRepository.AddAsync(skill);
                await _skillRepository.SaveChangesAsync();

                return CreatedAtAction(nameof(GetSkill), new { id = skill.Id }, skill);
            }
            catch (Exception e)
            {
                return BadRequest($"Erro ao cadastrar skill: {e.Message}");
            }
        }

        // DELETE: /Skill/{id}
        [HttpDelete("{id:guid}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> DeleteSkill(Guid id)
        {
            var skill = await _skillRepository.GetByIdAsync(id);
            if (skill == null)
                return NotFound("Skill não existe ou não foi encontrada");

            try
            {
                await _skillRepository.DeleteAsync(id);
                await _skillRepository.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception e)
            {
                return BadRequest($"Erro ao deletar a skill: {e.Message}");
            }
        }

        // GET: /Skill/paginado
        [HttpGet("paginado", Name = "GetSkillsPaged")]
        [Produces("application/hal+json")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPaged(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null,
            [FromQuery] string? sortDir = "Asc",
            CancellationToken ct = default)
        {
            var pr = await _skillsRepository.GetPaginationAsyncSkill(page, pageSize, ct);
            pr ??= new PageResult<Skill> { Items = Array.Empty<Skill>(), Page = page, PageSize = pageSize, Total = 0 };

            var items = pr.Items.Select(s => new Skill.SkillResponse(
                s.Id,
                s.Nome,
                s.Descricao
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
                    "GetSkillsPaged",
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
                _embedded = new { skills = items },
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
    }
}
