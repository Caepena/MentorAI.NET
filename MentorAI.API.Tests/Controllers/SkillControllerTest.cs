using FluentAssertions;
using MentorAI.API.Controllers;
using MentorAI.API.Models;
using MentorAI.Domain.Entities;
using MentorAI.Domain.Interfaces;
using MentorAI.Domain.Pagination;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Moq;


namespace MentorAI.API.Tests.Controllers
{
    public class SkillControllerTest
    {
        private readonly Mock<IRepository<Skill>> _repoMock;
        private readonly Mock<ISkillRepository> _repoPagedMock;
        private readonly LinkGenerator _linkGen;
        private readonly SkillController _controller;

        public SkillControllerTest()
        {
            _repoMock = new Mock<IRepository<Skill>>();
            _repoPagedMock = new Mock<ISkillRepository>();
            _linkGen = new Mock<LinkGenerator>().Object;

            _controller = new SkillController(
                _repoMock.Object,
                _linkGen,
                _repoPagedMock.Object);
        }

        [Fact]
        public async Task GetSkills_ReturnsOk_WithList()
        {
            // Arrange
            var expectedList = new List<Skill>
            {
                new Skill("Liderança", "Habilidade de liderar times")
            };

            _repoMock.Setup(r => r.GetAllAsync())
                .ReturnsAsync(expectedList);

            // Act
            var result = await _controller.GetSkills() as OkObjectResult;

            // Assert
            result.Should().NotBeNull();
            result!.StatusCode.Should().Be(StatusCodes.Status200OK);
            var value = result.Value as IEnumerable<Skill>;
            value.Should().NotBeNull();
            value!.Should().HaveCount(1);
            _repoMock.Verify(r => r.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetSkill_ReturnsOk_WhenFound()
        {
            // Arrange
            var skill = new Skill("Liderança", "Habilidade de liderar times");
            _repoMock.Setup(r => r.GetByIdAsync(skill.Id))
                .ReturnsAsync(skill);

            // Act
            var result = await _controller.GetSkill(skill.Id) as OkObjectResult;

            // Assert
            result.Should().NotBeNull();
            result!.Value.Should().Be(skill);
            result.StatusCode.Should().Be(StatusCodes.Status200OK);
            _repoMock.Verify(r => r.GetByIdAsync(skill.Id), Times.Once);
        }

        [Fact]
        public async Task GetSkill_ReturnsNotFound_WhenNull()
        {
            // Arrange
            var id = Guid.NewGuid();
            _repoMock.Setup(r => r.GetByIdAsync(id))
                .ReturnsAsync((Skill?)null);

            // Act
            var result = await _controller.GetSkill(id) as NotFoundObjectResult;

            // Assert
            result.Should().NotBeNull();
            result!.StatusCode.Should().Be(StatusCodes.Status404NotFound);
            _repoMock.Verify(r => r.GetByIdAsync(id), Times.Once);
        }

        [Fact]
        public async Task PostSkill_ReturnsCreated_WhenValid()
        {
            // Arrange
            var input = new SkillInputModel
            {
                Nome = "Liderança",
                Descricao = "Descrição"
            };

            _repoMock.Setup(r => r.AddAsync(It.IsAny<Skill>())).Returns(Task.CompletedTask);
            _repoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.PostSkill(input) as CreatedAtActionResult;

            // Assert
            result.Should().NotBeNull();
            result!.StatusCode.Should().Be(StatusCodes.Status201Created);
            result.Value.Should().BeOfType<Skill>();
            _repoMock.Verify(r => r.AddAsync(It.IsAny<Skill>()), Times.Once);
            _repoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task PutSkill_ReturnsNoContent_WhenUpdated()
        {
            // Arrange
            var id = Guid.NewGuid();
            var existing = new Skill("Liderança", "Desc");

            _repoMock.Setup(r => r.GetByIdAsync(id))
                .ReturnsAsync(existing);
            _repoMock.Setup(r => r.UpdateAsync(It.IsAny<Skill>())).Returns(Task.CompletedTask);
            _repoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            var input = new SkillInputModel
            {
                Nome = "Liderança Avançada",
                Descricao = "Nova desc"
            };

            // Act
            var result = await _controller.PutSkill(id, input) as NoContentResult;

            // Assert
            result.Should().NotBeNull();
            result!.StatusCode.Should().Be(StatusCodes.Status204NoContent);
            _repoMock.Verify(r => r.UpdateAsync(It.IsAny<Skill>()), Times.Once);
            _repoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task PutSkill_ReturnsNotFound_WhenNotExists()
        {
            // Arrange
            var id = Guid.NewGuid();
            _repoMock.Setup(r => r.GetByIdAsync(id))
                .ReturnsAsync((Skill?)null);

            var input = new SkillInputModel
            {
                Nome = "Liderança",
                Descricao = "Desc"
            };

            // Act
            var result = await _controller.PutSkill(id, input) as NotFoundObjectResult;

            // Assert
            result.Should().NotBeNull();
            result!.StatusCode.Should().Be(StatusCodes.Status404NotFound);
            _repoMock.Verify(r => r.GetByIdAsync(id), Times.Once);
        }

        [Fact]
        public async Task DeleteSkill_ReturnsNoContent_WhenDeleted()
        {
            // Arrange
            var id = Guid.NewGuid();
            var skill = new Skill("Liderança", "Desc");

            _repoMock.Setup(r => r.GetByIdAsync(id))
                .ReturnsAsync(skill);
            _repoMock.Setup(r => r.DeleteAsync(id)).Returns(Task.CompletedTask);
            _repoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteSkill(id) as NoContentResult;

            // Assert
            result.Should().NotBeNull();
            result!.StatusCode.Should().Be(StatusCodes.Status204NoContent);
            _repoMock.Verify(r => r.DeleteAsync(id), Times.Once);
            _repoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteSkill_ReturnsNotFound_WhenNotExists()
        {
            // Arrange
            var id = Guid.NewGuid();
            _repoMock.Setup(r => r.GetByIdAsync(id))
                .ReturnsAsync((Skill?)null);

            // Act
            var result = await _controller.DeleteSkill(id) as NotFoundObjectResult;

            // Assert
            result.Should().NotBeNull();
            result!.StatusCode.Should().Be(StatusCodes.Status404NotFound);
            _repoMock.Verify(r => r.GetByIdAsync(id), Times.Once);
        }

        [Fact]
        public async Task GetPaged_ReturnsOk_WithValidData()
        {
            // Arrange
            var skill = new Skill("Liderança", "Desc");
            var pageResult = new PageResult<Skill>
            {
                Items = new List<Skill> { skill },
                Page = 1,
                PageSize = 10,
                Total = 1
            };

            _repoPagedMock.Setup(r => r.GetPaginationAsyncSkill(1, 10, It.IsAny<CancellationToken>()))
                .ReturnsAsync(pageResult);

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Scheme = "http";
            httpContext.Request.Host = new HostString("localhost", 5000);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = await _controller.GetPaged(1, 10) as OkObjectResult;

            // Assert
            result.Should().NotBeNull();
            result!.StatusCode.Should().Be(StatusCodes.Status200OK);
            _repoPagedMock.Verify(r => r.GetPaginationAsyncSkill(1, 10, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
