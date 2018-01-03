using AutoMapper;
using FluentAssertions;
using Gugleus.Api.Controllers;
using Gugleus.Core.Dto.Input;
using Gugleus.Core.Dto.Output;
using Gugleus.Core.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace Gugleus.Tests.Controllers
{
    public class PostsControllerTests
    {
        private readonly Mock<IRequestService> _postServiceMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<PostsController>> _loggerMock;
        private readonly Mock<IHttpContextAccessor> _httpContextMock;
        private readonly PostsController _controller;

        public PostsControllerTests()
        {
            _postServiceMock = new Mock<IRequestService>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<PostsController>>();
            _httpContextMock = new Mock<IHttpContextAccessor>();
            _controller = new PostsController(_postServiceMock.Object, _mapperMock.Object,
                _loggerMock.Object, _httpContextMock.Object);
        }

        [Fact(DisplayName = "Ping")]
        public void Ping()
        {
            // Arrange

            // Act
            IActionResult actionResult = _controller.Ping();

            // Assert
            actionResult.Should().NotBeNull().And.BeOfType<OkObjectResult>();
        }


        [Theory(DisplayName = "PostNullInput")]
        [InlineData(null)]
        public void PostNullInput(PostDto newPost)
        {
            // Arrange

            // Act
            Task<IActionResult> taskWithActionResult = _controller.AddPost(newPost);

            // Assert
            _postServiceMock.Verify(x => x.AddRequestAsync(It.IsAny<PostDto>()), Times.Never);

            taskWithActionResult.Result.Should().NotBeNull().And.BeOfType<BadRequestObjectResult>();

            var badRequestResult = taskWithActionResult.Result as BadRequestObjectResult;
            badRequestResult.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
            badRequestResult.Value.Should().BeOfType<ResultDto>();
            var result = badRequestResult.Value as ResultDto;
            result.IsOk.Should().Be(false);
            result.Message.Should().Be("Null input.");
        }
    }
}
