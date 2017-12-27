using FluentAssertions;
using Gugleus.Api.Controllers;
using Gugleus.Core.Dto;
using Gugleus.Core.Repositories;
using Gugleus.Core.Results;
using Gugleus.Core.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace Gugleus.Tests.Controllers
{
    public class PostsControllerTests
    {
        private readonly Mock<IPostService> _postServiceMock;
        private readonly Mock<IValidationService> _validationServiceMock;
        private readonly PostsController _controller;

        public PostsControllerTests()
        {
            _postServiceMock = new Mock<IPostService>();
            _validationServiceMock = new Mock<IValidationService>();
            _controller = new PostsController(_postServiceMock.Object, _validationServiceMock.Object);
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
        public void PostNullInput(PostDto newPost)
        {
            // Arrange
            _validationServiceMock.Setup(x => x.ValidateNewPost(newPost))
                .Returns(new MessageListResult() { IsOk = false });

            // Act
            Task<IActionResult> taskWithActionResult = _controller.Post(newPost);

            // Assert
            _postServiceMock.Verify(x => x.AddPost(It.IsAny<PostDto>()), Times.Never);

            taskWithActionResult.Result.Should().NotBeNull().And.BeOfType<BadRequestObjectResult>();

            var badRequestResult = taskWithActionResult.Result as BadRequestObjectResult;
            badRequestResult.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
            badRequestResult.Value.Should().Be("Null input.");
        }
    }
}
