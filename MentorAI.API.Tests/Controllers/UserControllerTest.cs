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
    public class UserControllerTest
    {
        private readonly Mock<IRepository<User>> _repoMock;
        private readonly Mock<IUserRepository> _repoPagedMock;
        private readonly Mock<IUserCourseUseCase> _userCourseUseCaseMock;
        private readonly LinkGenerator _linkGen;
        private readonly UserController _controller;

        public UserControllerTest()
        {
            _repoMock = new Mock<IRepository<User>>();
            _repoPagedMock = new Mock<IUserRepository>();
            _userCourseUseCaseMock = new Mock<IUserCourseUseCase>();
            _linkGen = new Mock<LinkGenerator>().Object;

            _controller = new UserController(
                _repoMock.Object,
                _linkGen,
                _repoPagedMock.Object,
                _userCourseUseCaseMock.Object);
        }

        [Fact]
        public async Task GetUsers_ReturnsOk_WithList()
        {
            // Arrange
            var expectedList = new List<User>
            {
                new User("Fulano", "fulano@teste.com", "Dev Jr", "Dev Pleno")
            };

            _repoMock.Setup(r => r.GetAllAsync())
                .ReturnsAsync(expectedList);

            // Act
            var result = await _controller.GetUsers() as OkObjectResult;

            // Assert
            result.Should().NotBeNull();
            result!.StatusCode.Should().Be(StatusCodes.Status200OK);
            var value = result.Value as IEnumerable<User>;
            value.Should().NotBeNull();
            value!.Should().HaveCount(1);
            _repoMock.Verify(r => r.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetUser_ReturnsOk_WhenFound()
        {
            // Arrange
            var user = new User("Fulano", "fulano@teste.com", "Dev Jr", "Dev Pleno");
            _repoMock.Setup(r => r.GetByIdAsync(user.Id))
                .ReturnsAsync(user);

            // Act
            var result = await _controller.GetUser(user.Id) as OkObjectResult;

            // Assert
            result.Should().NotBeNull();
            result!.Value.Should().Be(user);
            result.StatusCode.Should().Be(StatusCodes.Status200OK);
            _repoMock.Verify(r => r.GetByIdAsync(user.Id), Times.Once);
        }

        [Fact]
        public async Task GetUser_ReturnsNotFound_WhenNull()
        {
            // Arrange
            var id = Guid.NewGuid();
            _repoMock.Setup(r => r.GetByIdAsync(id))
                .ReturnsAsync((User?)null);

            // Act
            var result = await _controller.GetUser(id) as NotFoundObjectResult;

            // Assert
            result.Should().NotBeNull();
            result!.StatusCode.Should().Be(StatusCodes.Status404NotFound);
            _repoMock.Verify(r => r.GetByIdAsync(id), Times.Once);
        }

        [Fact]
        public async Task PostUser_ReturnsCreated_WhenValid()
        {
            // Arrange
            var input = new UserInputModel
            {
                Nome = "Fulano",
                Email = "fulano@teste.com",
                CargoAtual = "Dev Jr",
                CargoDesejado = "Dev Pleno"
            };

            _repoMock.Setup(r => r.AddAsync(It.IsAny<User>())).Returns(Task.CompletedTask);
            _repoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.PostUser(input) as CreatedAtActionResult;

            // Assert
            result.Should().NotBeNull();
            result!.StatusCode.Should().Be(StatusCodes.Status201Created);
            result.Value.Should().BeOfType<User>();
            _repoMock.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Once);
            _repoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task PutUser_ReturnsNoContent_WhenUpdated()
        {
            // Arrange
            var id = Guid.NewGuid();
            var existing = new User("Fulano", "fulano@teste.com", "Dev Jr", "Dev Pleno");

            _repoMock.Setup(r => r.GetByIdAsync(id))
                .ReturnsAsync(existing);
            _repoMock.Setup(r => r.UpdateAsync(It.IsAny<User>())).Returns(Task.CompletedTask);
            _repoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            var input = new UserInputModel
            {
                Nome = "Fulano Atualizado",
                Email = "fulano@teste.com",
                CargoAtual = "Dev Pleno",
                CargoDesejado = "Tech Lead"
            };

            // Act
            var result = await _controller.PutUser(id, input) as NoContentResult;

            // Assert
            result.Should().NotBeNull();
            result!.StatusCode.Should().Be(StatusCodes.Status204NoContent);
            _repoMock.Verify(r => r.UpdateAsync(It.IsAny<User>()), Times.Once);
            _repoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task PutUser_ReturnsNotFound_WhenNotExists()
        {
            // Arrange
            var id = Guid.NewGuid();
            _repoMock.Setup(r => r.GetByIdAsync(id))
                .ReturnsAsync((User?)null);

            var input = new UserInputModel
            {
                Nome = "Fulano",
                Email = "fulano@teste.com",
                CargoAtual = "Dev Jr",
                CargoDesejado = "Dev Pleno"
            };

            // Act
            var result = await _controller.PutUser(id, input) as NotFoundObjectResult;

            // Assert
            result.Should().NotBeNull();
            result!.StatusCode.Should().Be(StatusCodes.Status404NotFound);
            _repoMock.Verify(r => r.GetByIdAsync(id), Times.Once);
        }

        [Fact]
        public async Task DeleteUser_ReturnsNoContent_WhenDeleted()
        {
            // Arrange
            var id = Guid.NewGuid();
            var user = new User("Fulano", "fulano@teste.com", "Dev Jr", "Dev Pleno");

            _repoMock.Setup(r => r.GetByIdAsync(id))
                .ReturnsAsync(user);
            _repoMock.Setup(r => r.DeleteAsync(id)).Returns(Task.CompletedTask);
            _repoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteUser(id) as NoContentResult;

            // Assert
            result.Should().NotBeNull();
            result!.StatusCode.Should().Be(StatusCodes.Status204NoContent);
            _repoMock.Verify(r => r.DeleteAsync(id), Times.Once);
            _repoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteUser_ReturnsNotFound_WhenNotExists()
        {
            // Arrange
            var id = Guid.NewGuid();
            _repoMock.Setup(r => r.GetByIdAsync(id))
                .ReturnsAsync((User?)null);

            // Act
            var result = await _controller.DeleteUser(id) as NotFoundObjectResult;

            // Assert
            result.Should().NotBeNull();
            result!.StatusCode.Should().Be(StatusCodes.Status404NotFound);
            _repoMock.Verify(r => r.GetByIdAsync(id), Times.Once);
        }

        [Fact]
        public async Task GetPaged_ReturnsOk_WithValidData()
        {
            // Arrange
            var user = new User("Fulano", "fulano@teste.com", "Dev Jr", "Dev Pleno");
            var pageResult = new PageResult<User>
            {
                Items = new List<User> { user },
                Page = 1,
                PageSize = 10,
                Total = 1
            };

            _repoPagedMock.Setup(r => r.GetPaginationAsyncUser(1, 10, It.IsAny<CancellationToken>()))
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
            _repoPagedMock.Verify(r => r.GetPaginationAsyncUser(1, 10, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
