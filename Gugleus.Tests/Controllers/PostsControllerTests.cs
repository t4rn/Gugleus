using AutoMapper;
using FluentAssertions;
using Gugleus.Api.Controllers;
using Gugleus.Core.Domain;
using Gugleus.Core.Dto.Input;
using Gugleus.Core.Dto.Output;
using Gugleus.Core.Services;
using Gugleus.GoogleCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Internal;
using Moq;
using System;
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

        [Fact(DisplayName = "GetPostStatusBadRequest")]
        public async void GetPostStatusBadRequest()
        {
            // Arrange
            long postId = new Random().Next(int.MaxValue);
            _postServiceMock
                .Setup(x => x.GetRequestResponseAsync<GoogleInfo>(postId, DictionaryItem.RequestType.ADDPOST))
                .ReturnsAsync((RequestResponseDto<GoogleInfo>)null);

            // Act
            IActionResult actionResult = await _controller.GetPostStatus(postId);

            // Assert
            _postServiceMock.Verify(
                x => x.GetRequestResponseAsync<GoogleInfo>(postId, DictionaryItem.RequestType.ADDPOST),
                Times.Once);
            _loggerMock.Verify(m => m.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<FormattedLogValues>(v => v.ToString().Contains("not found")),
                It.IsAny<Exception>(),
                It.IsAny<Func<object, Exception, string>>()
                ));

            actionResult.Should().NotBeNull().And.BeOfType<BadRequestObjectResult>();

            var badRequestResult = actionResult as BadRequestObjectResult;
            badRequestResult.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
            badRequestResult.Value.Should().NotBeNull().And.BeOfType<string>().And.Be($"Post with Id: '{postId}' not found...");
        }


        [Theory(DisplayName = "PostNullInput")]
        [InlineData(null)]
        public async void PostNullInput(PostDto newPost)
        {
            // Arrange

            // Act
            IActionResult actionResult = await _controller.AddPost(newPost);

            // Assert
            _postServiceMock.Verify(x => x.AddRequestAsync(It.IsAny<PostDto>()), Times.Never);

            actionResult.Should().NotBeNull().And.BeOfType<BadRequestObjectResult>();

            var badRequestResult = actionResult as BadRequestObjectResult;
            badRequestResult.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
            badRequestResult.Value.Should().BeOfType<ResultDto>();
            var result = badRequestResult.Value as ResultDto;
            result.IsOk.Should().Be(false);
            result.Message.Should().Be("Null input.");
        }
    }
}
