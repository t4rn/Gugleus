using FluentAssertions;
using Gugleus.Api.Controllers;
using Gugleus.Core.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace Gugleus.Tests.Controllers
{
    public class PostsControllerTests
    {
        private readonly Mock<IPostRepository> _postRepositoryMock;
        private readonly PostsController _controller;

        public PostsControllerTests()
        {
            _postRepositoryMock = new Mock<IPostRepository>();
            _controller = new PostsController(_postRepositoryMock.Object);
        }

        [Fact(DisplayName = "GetPing")]
        public void GetPing()
        {
            // Arrange

            // Act
            IActionResult actionResult = _controller.Get();

            // Assert
            actionResult.Should().NotBeNull().And.BeOfType<OkObjectResult>();
        }


        [Theory(DisplayName = "PostNullInput")]
        [InlineData(null)]
        public void PostNullInput(string newPost)
        {
            // Arrange

            // Act
            Task<IActionResult> taskWithActionResult = _controller.Post(newPost);

            // Assert
            _postRepositoryMock.Verify(x => x.AddPost(It.IsAny<string>()), Times.Never);

            taskWithActionResult.Result.Should().NotBeNull().And.BeOfType<BadRequestObjectResult>();

            var badRequestResult = taskWithActionResult.Result as BadRequestObjectResult;
            badRequestResult.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
            badRequestResult.Value.Should().Be("Null input.");
        }
    }
}
