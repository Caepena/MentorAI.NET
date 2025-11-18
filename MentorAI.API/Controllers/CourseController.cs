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
    public class CourseController : ControllerBase
    {
        private readonly LinkGenerator _links;
        private readonly ICourseRepository _coursesRepository;
        private readonly IRepository<Course> _courseRepository;

        public CourseController(
            IRepository<Course> courseRepository,
            LinkGenerator links,
            ICourseRepository coursesRepository)
        {
            _courseRepository = courseRepository;
            _coursesRepository = coursesRepository;
            _links = links ?? throw new ArgumentException(nameof(links));
        }

        // GET: /Course
        [HttpGet]
        public async Task<IActionResult> GetCourses(CancellationToken ct)
        {
            var courses = await _coursesRepository.GetAllWithRelationsAsync(ct);
            return Ok(courses);
        }

        // GET: /Course/{id}
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetCourse(Guid id, CancellationToken ct)
        {
            var course = await _coursesRepository.GetByIdWithRelationsAsync(id, ct);
            if (course == null)
                return NotFound("Curso não existe ou não foi encontrado");

            return Ok(course);
        }

        // PUT: /Course/{id}
        [HttpPut("{id:guid}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> PutCourse(Guid id, [FromBody] CourseInputModel inputModel)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var courseExiste = await _courseRepository.GetByIdAsync(id);
            if (courseExiste == null)
                return NotFound("Curso não existe ou não foi encontrado");

            try
            {
                courseExiste.Refresh(
                    titulo: inputModel.Titulo,
                    descricao: inputModel.Descricao,
                    provedor: inputModel.Provedor,
                    cargaHoraria: inputModel.CargaHoraria
                );

                courseExiste.DefinirSkill(inputModel.SkillId);

                await _courseRepository.UpdateAsync(courseExiste);
                await _courseRepository.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Erro ao atualizar o curso: {e.Message}");
            }
        }

        // POST: /Course
        [HttpPost]
        [ProducesResponseType(typeof(Course), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> PostCourse([FromBody] CourseInputModel inputModel)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var course = new Course(
                    titulo: inputModel.Titulo,
                    descricao: inputModel.Descricao,
                    provedor: inputModel.Provedor,
                    cargaHoraria: inputModel.CargaHoraria,
                    skillId: inputModel.SkillId
                );

                await _courseRepository.AddAsync(course);
                await _courseRepository.SaveChangesAsync();

                return CreatedAtAction(nameof(GetCourse), new { id = course.Id }, course);
            }
            catch (Exception e)
            {
                return BadRequest($"Erro ao cadastrar curso: {e.Message}");
            }
        }

        // DELETE: /Course/{id}
        [HttpDelete("{id:guid}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> DeleteCourse(Guid id)
        {
            var course = await _courseRepository.GetByIdAsync(id);
            if (course == null)
                return NotFound("Curso não existe ou não foi encontrado");

            try
            {
                await _courseRepository.DeleteAsync(id);
                await _courseRepository.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception e)
            {
                return BadRequest($"Erro ao deletar o curso: {e.Message}");
            }
        }

        // GET: /Course/paginado
        [HttpGet("paginado", Name = "GetCoursesPaged")]
        [Produces("application/hal+json")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPaged(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null,
            [FromQuery] string? sortDir = "Asc",
            CancellationToken ct = default)
        {
            var pr = await _coursesRepository.GetPaginationAsyncCourse(page, pageSize, ct);
            pr ??= new PageResult<Course>
            {
                Items = Array.Empty<Course>(),
                Page = page,
                PageSize = pageSize,
                Total = 0
            };

            var items = pr.Items.Select(c => new Course.CursoResponse(
                c.Id,
                c.Titulo,
                c.Descricao,
                c.Provedor,
                c.CargaHoraria,
                c.SkillId
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
                    "GetCoursesPaged",
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
                _embedded = new { cursos = items },
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
