using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
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
using Xunit;

namespace MentorAI.API.Tests.Controllers
{
    public class CourseControllerTest
    {
        private readonly Mock<IRepository<Course>> _repoMock;
        private readonly Mock<ICourseRepository> _repoPagedMock;
        private readonly LinkGenerator _linkGen;
        private readonly CourseController _controller;

        public CourseControllerTest()
        {
            _repoMock = new Mock<IRepository<Course>>();
            _repoPagedMock = new Mock<ICourseRepository>();
            _linkGen = new Mock<LinkGenerator>().Object;

            _controller = new CourseController(
                _repoMock.Object,
                _linkGen,
                _repoPagedMock.Object);
        }

        [Fact]
        public async Task GetCourses_ReturnsOk_WithList()
        {
            // Arrange
            var skillId = Guid.NewGuid();
            var expectedList = new List<Course>
            {
                new Course("Curso .NET", "Desc", "FIAP", 40, skillId)
            };

            _repoPagedMock.Setup(r => r.GetAllWithRelationsAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedList);

            // Act
            var result = await _controller.GetCourses(CancellationToken.None) as OkObjectResult;

            // Assert
            result.Should().NotBeNull();
            result!.StatusCode.Should().Be(StatusCodes.Status200OK);
            var value = result.Value as IEnumerable<Course>;
            value.Should().NotBeNull();
            value!.Should().HaveCount(1);
            _repoPagedMock.Verify(r => r.GetAllWithRelationsAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetCourse_ReturnsOk_WhenFound()
        {
            // Arrange
            var skillId = Guid.NewGuid();
            var course = new Course("Curso .NET", "Desc", "FIAP", 40, skillId);

            _repoPagedMock // <- usa ICourseRepository, que é o _coursesRepository
                .Setup(r => r.GetByIdWithRelationsAsync(course.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(course);

            // Act
            var result = await _controller.GetCourse(course.Id, CancellationToken.None) as OkObjectResult;

            // Assert
            result.Should().NotBeNull();
            result!.Value.Should().Be(course);
            result.StatusCode.Should().Be(StatusCodes.Status200OK);
            _repoPagedMock.Verify(
                r => r.GetByIdWithRelationsAsync(course.Id, It.IsAny<CancellationToken>()),
                Times.Once);
        }


        [Fact]
        public async Task GetCourse_ReturnsNotFound_WhenNull()
        {
            // Arrange
            var id = Guid.NewGuid();

            _repoPagedMock
                .Setup(r => r.GetByIdWithRelationsAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Course?)null);

            // Act
            var result = await _controller.GetCourse(id, CancellationToken.None) as NotFoundObjectResult;

            // Assert
            result.Should().NotBeNull();
            result!.StatusCode.Should().Be(StatusCodes.Status404NotFound);
            _repoPagedMock.Verify(
                r => r.GetByIdWithRelationsAsync(id, It.IsAny<CancellationToken>()),
                Times.Once);
        }


        [Fact]
        public async Task PostCourse_ReturnsCreated_WhenValid()
        {
            // Arrange
            var input = new CourseInputModel
            {
                Titulo = "Curso .NET",
                Descricao = "Desc",
                Provedor = "FIAP",
                CargaHoraria = 40,
                SkillId = Guid.NewGuid()
            };

            _repoMock.Setup(r => r.AddAsync(It.IsAny<Course>())).Returns(Task.CompletedTask);
            _repoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.PostCourse(input) as CreatedAtActionResult;

            // Assert
            result.Should().NotBeNull();
            result!.StatusCode.Should().Be(StatusCodes.Status201Created);
            result.Value.Should().BeOfType<Course>();
            _repoMock.Verify(r => r.AddAsync(It.IsAny<Course>()), Times.Once);
            _repoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task PutCourse_ReturnsNoContent_WhenUpdated()
        {
            // Arrange
            var id = Guid.NewGuid();
            var skillId = Guid.NewGuid();
            var existing = new Course("Curso .NET", "Desc", "FIAP", 40, skillId);

            _repoMock.Setup(r => r.GetByIdAsync(id))
                .ReturnsAsync(existing);
            _repoMock.Setup(r => r.UpdateAsync(It.IsAny<Course>())).Returns(Task.CompletedTask);
            _repoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            var input = new CourseInputModel
            {
                Titulo = "Curso .NET Avançado",
                Descricao = "Nova desc",
                Provedor = "FIAP",
                CargaHoraria = 60,
                SkillId = skillId
            };

            // Act
            var result = await _controller.PutCourse(id, input) as NoContentResult;

            // Assert
            result.Should().NotBeNull();
            result!.StatusCode.Should().Be(StatusCodes.Status204NoContent);
            _repoMock.Verify(r => r.UpdateAsync(It.IsAny<Course>()), Times.Once);
            _repoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task PutCourse_ReturnsNotFound_WhenNotExists()
        {
            // Arrange
            var id = Guid.NewGuid();
            _repoMock.Setup(r => r.GetByIdAsync(id))
                .ReturnsAsync((Course?)null);

            var input = new CourseInputModel
            {
                Titulo = "Curso .NET",
                Descricao = "Desc",
                Provedor = "FIAP",
                CargaHoraria = 40,
                SkillId = Guid.NewGuid()
            };

            // Act
            var result = await _controller.PutCourse(id, input) as NotFoundObjectResult;

            // Assert
            result.Should().NotBeNull();
            result!.StatusCode.Should().Be(StatusCodes.Status404NotFound);
            _repoMock.Verify(r => r.GetByIdAsync(id), Times.Once);
        }

        [Fact]
        public async Task DeleteCourse_ReturnsNoContent_WhenDeleted()
        {
            // Arrange
            var id = Guid.NewGuid();
            var skillId = Guid.NewGuid();
            var course = new Course("Curso .NET", "Desc", "FIAP", 40, skillId);

            _repoMock.Setup(r => r.GetByIdAsync(id))
                .ReturnsAsync(course);
            _repoMock.Setup(r => r.DeleteAsync(id)).Returns(Task.CompletedTask);
            _repoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteCourse(id) as NoContentResult;

            // Assert
            result.Should().NotBeNull();
            result!.StatusCode.Should().Be(StatusCodes.Status204NoContent);
            _repoMock.Verify(r => r.DeleteAsync(id), Times.Once);
            _repoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteCourse_ReturnsNotFound_WhenNotExists()
        {
            // Arrange
            var id = Guid.NewGuid();
            _repoMock.Setup(r => r.GetByIdAsync(id))
                .ReturnsAsync((Course?)null);

            // Act
            var result = await _controller.DeleteCourse(id) as NotFoundObjectResult;

            // Assert
            result.Should().NotBeNull();
            result!.StatusCode.Should().Be(StatusCodes.Status404NotFound);
            _repoMock.Verify(r => r.GetByIdAsync(id), Times.Once);
        }

        [Fact]
        public async Task GetPaged_ReturnsOk_WithValidData()
        {
            // Arrange
            var skillId = Guid.NewGuid();
            var course = new Course("Curso .NET", "Desc", "FIAP", 40, skillId);
            var pageResult = new PageResult<Course>
            {
                Items = new List<Course> { course },
                Page = 1,
                PageSize = 10,
                Total = 1
            };

            _repoPagedMock.Setup(r => r.GetPaginationAsyncCourse(1, 10, It.IsAny<CancellationToken>()))
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
            _repoPagedMock.Verify(r => r.GetPaginationAsyncCourse(1, 10, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
