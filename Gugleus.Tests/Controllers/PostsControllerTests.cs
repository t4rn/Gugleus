using AutoMapper;
using FluentAssertions;
using Gugleus.Api.Controllers;
using Gugleus.Core.Domain;
using Gugleus.Core.Dto.Input;
using Gugleus.Core.Dto.Output;
using Gugleus.Core.Results;
using Gugleus.Core.Services;
using Gugleus.GoogleCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Internal;
using Moq;
using System;
using System.Collections.Generic;
using System.Net;
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
        private readonly Mock<ICacheService> _cacheServiceMock;
        private readonly PostsController _controller;

        public PostsControllerTests()
        {
            _postServiceMock = new Mock<IRequestService>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<PostsController>>();
            _httpContextMock = new Mock<IHttpContextAccessor>();
            _cacheServiceMock = new Mock<ICacheService>();
            _controller = new PostsController(_postServiceMock.Object, _mapperMock.Object,
                _loggerMock.Object, _httpContextMock.Object, _cacheServiceMock.Object);
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
            var requestType = DictionaryItem.RequestType.ADDPOST;
            _postServiceMock
                .Setup(x => x.GetRequestResponseAsync<GoogleInfo>(postId, requestType))
                .ReturnsAsync((RequestResponseDto<GoogleInfo>)null);

            // Act
            IActionResult actionResult = await _controller.GetPostStatus(postId);

            // Assert
            _postServiceMock.Verify(
                x => x.GetRequestResponseAsync<GoogleInfo>(postId, requestType),
                Times.Once);
            _loggerMock.Verify(m => m.Log(
                LogLevel.Error,
                0,
                It.Is<FormattedLogValues>(v => v.ToString()
                .Contains($"Post with Id: '{postId}' and type '{requestType}' not found")),
                null,
                It.IsAny<Func<object, Exception, string>>()
                ));

            actionResult.Should().NotBeNull().And.BeOfType<BadRequestObjectResult>();

            var badRequestResult = actionResult as BadRequestObjectResult;
            badRequestResult.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
            badRequestResult.Value.Should().NotBeNull().And.BeOfType<string>().And.Be($"Post with Id: '{postId}' not found...");
        }

        [Fact(DisplayName = "GetPostStatusInternalServerError")]
        public async void GetPostStatusInternalServerError()
        {
            // Arrange
            long postId = new Random().Next(int.MaxValue);
            var requestType = DictionaryItem.RequestType.ADDPOST;
            string expectedError = Guid.NewGuid().ToString();
            var expectedResponse = new RequestResponseDto<GoogleInfo>() { Error = expectedError };
            _postServiceMock
                .Setup(x => x.GetRequestResponseAsync<GoogleInfo>(postId, requestType))
                .ReturnsAsync(expectedResponse);

            // Act
            IActionResult actionResult = await _controller.GetPostStatus(postId);

            // Assert
            _postServiceMock.Verify(
                x => x.GetRequestResponseAsync<GoogleInfo>(postId, requestType),
                Times.Once);
            _loggerMock.Verify(m => m.Log(
                LogLevel.Error,
                0,
                It.Is<FormattedLogValues>(v => v.ToString()
                .Contains($"Error for Id: '{postId}' type: '{requestType}' -> {expectedError}")),
                null,
                It.IsAny<Func<object, Exception, string>>()
                ));

            actionResult.Should().NotBeNull().And.BeOfType<ObjectResult>();

            var objectResult = actionResult as ObjectResult;
            objectResult.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
            objectResult.Value.Should().NotBeNull().And.BeOfType<RequestResponseDto<GoogleInfo>>();
            var requestResponse = objectResult.Value as RequestResponseDto<GoogleInfo>;
            requestResponse.Error.Should().Be(expectedError);
        }

        [Fact(DisplayName = "GetPostStatusOk")]
        public async void GetPostStatusOk()
        {
            // Arrange
            long postId = new Random().Next(int.MaxValue);
            var requestType = DictionaryItem.RequestType.ADDPOST;
            string expectedRequestStatus = Guid.NewGuid().ToString();
            var expectedResponse = new RequestResponseDto<GoogleInfo>() { Id = postId, Status = expectedRequestStatus };
            _postServiceMock
                .Setup(x => x.GetRequestResponseAsync<GoogleInfo>(postId, requestType))
                .ReturnsAsync(expectedResponse);

            // Act
            IActionResult actionResult = await _controller.GetPostStatus(postId);

            // Assert
            _postServiceMock.Verify(
                x => x.GetRequestResponseAsync<GoogleInfo>(postId, requestType),
                Times.Once);
            _loggerMock.Verify(m => m.Log(
                LogLevel.Debug,
                0,
                It.Is<FormattedLogValues>(v => v.ToString()
                .Contains($"Ok for Id: '{postId}' type: '{requestType}' -> {expectedRequestStatus}")),
                null,
                It.IsAny<Func<object, Exception, string>>()
                ));

            actionResult.Should().NotBeNull().And.BeOfType<OkObjectResult>();

            var objectResult = actionResult as OkObjectResult;
            objectResult.StatusCode.Should().Be(StatusCodes.Status200OK);
            objectResult.Value.Should().NotBeNull().And.BeOfType<RequestResponseDto<GoogleInfo>>();
            var requestResponse = objectResult.Value as RequestResponseDto<GoogleInfo>;
            requestResponse.Error.Should().BeNull();
            requestResponse.Id.Should().Be(postId);
            requestResponse.Status.Should().Be(expectedRequestStatus);
        }

        [Fact(DisplayName = "AddPostNullInput")]
        public async void AddPostNullInput()
        {
            // Arrange

            // Act
            IActionResult actionResult = await _controller.AddPost(null);

            // Assert
            _cacheServiceMock.Verify(x => x.GetWsClientsAsync(), Times.Never);
            _postServiceMock.Verify(x => x.AddRequestAsync(It.IsAny<PostDto>(), It.IsAny<WsClient>()), Times.Never);
            _loggerMock.Verify(m => m.Log(
                LogLevel.Error,
                0,
                It.Is<FormattedLogValues>(v => v.ToString()
                .Contains($"Null input for '{typeof(PostDto)}'")),
                null,
                It.IsAny<Func<object, Exception, string>>()
                ));

            actionResult.Should().NotBeNull().And.BeOfType<BadRequestObjectResult>();

            var badRequestResult = actionResult as BadRequestObjectResult;
            badRequestResult.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
            badRequestResult.Value.Should().NotBeNull().And.BeOfType<ResultDto>();
            var resultDto = badRequestResult.Value as ResultDto;
            resultDto.IsOk.Should().Be(false);
            resultDto.Message.Should().NotBeNull().And.Be("Null input.");
        }

        [Fact(DisplayName = "AddPostNotValid")]
        public async void AddPostNotValid()
        {
            // Arrange
            PostDto postDto = new PostDto();
            string expectedErrorMsg = "Validation errors: Null User data.;Null Content data";
            ResultDto expectedResult = new ResultDto() { Message = expectedErrorMsg };
            _mapperMock.Setup(x => x.Map<ResultDto>(It.IsAny<MessageListResult>()))
                .Returns(expectedResult);

            // Act
            IActionResult actionResult = await _controller.AddPost(postDto);

            // Assert
            _cacheServiceMock.Verify(x => x.GetWsClientsAsync(), Times.Never);
            _postServiceMock.Verify(x => x.AddRequestAsync(postDto, It.IsAny<WsClient>()), Times.Never);
            _loggerMock.Verify(m => m.Log(
                LogLevel.Error,
                0,
                It.Is<FormattedLogValues>(v => v.ToString()
                .Contains($"ValidErr for '{postDto.GetType()}': {expectedErrorMsg}")),
                null,
                It.IsAny<Func<object, Exception, string>>()
                ));
            _mapperMock.Verify(x => x.Map<ResultDto>(It.IsAny<MessageListResult>()), Times.Once);

            actionResult.Should().NotBeNull().And.BeOfType<BadRequestObjectResult>();

            var badRequestResult = actionResult as BadRequestObjectResult;
            badRequestResult.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
            badRequestResult.Value.Should().NotBeNull().And.BeOfType<ResultDto>();
            var resultDto = badRequestResult.Value as ResultDto;
            resultDto.IsOk.Should().Be(false);
            resultDto.Message.Should().NotBeNull().And.Be(expectedErrorMsg);
        }

        [Fact(DisplayName = "AddPostNotOk")]
        public async void AddPostNotOk()
        {
            // Arrange
            List<WsClient> expectedWsClients = new List<WsClient>() { new WsClient { Hash = Guid.NewGuid().ToString(), Id = new Random().Next(int.MaxValue) } };
            PostDto postDto = new PostDto() { Content = Guid.NewGuid().ToString(), User = new UserInfoDto() };
            IdResultDto<long> expectedResult = new IdResultDto<long>() { IsOk = false, Message = Guid.NewGuid().ToString() };
            _postServiceMock.Setup(x => x.AddRequestAsync(postDto, It.IsAny<WsClient>())).ReturnsAsync(expectedResult);
            _cacheServiceMock.Setup(x => x.GetWsClientsAsync()).ReturnsAsync(expectedWsClients);

            // Act
            IActionResult actionResult = await _controller.AddPost(postDto);

            // Assert
            actionResult.Should().NotBeNull().And.BeOfType<ObjectResult>();

            _cacheServiceMock.Verify(x => x.GetWsClientsAsync(), Times.Once);
            _postServiceMock.Verify(x => x.AddRequestAsync(postDto, It.IsAny<WsClient>()), Times.Once);
            _loggerMock.Verify(m => m.Log(
                LogLevel.Error,
                0,
                It.Is<FormattedLogValues>(v => v.ToString()
                .Contains($"Error for: '{typeof(PostDto)}' -> Message: '{expectedResult.Message}'")),
                null,
                It.IsAny<Func<object, Exception, string>>()
                ));

            var objectResult = actionResult as ObjectResult;
            objectResult.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
            objectResult.Value.Should().NotBeNull().And.BeOfType(typeof(IdResultDto<long>));
            var idResultDto = objectResult.Value as IdResultDto<long>;
            idResultDto.IsOk.Should().Be(false);
            idResultDto.Message.Should().NotBeNull().And.Be(expectedResult.Message);
            idResultDto.Id.Should().Be(0);
        }

        [Fact(DisplayName = "AddPostOk")]
        public async void AddPostOk()
        {
            // Arrange
            List<WsClient> expectedWsClients = new List<WsClient>() { new WsClient { Hash = Guid.NewGuid().ToString(), Id = new Random().Next(int.MaxValue) } };
            PostDto postDto = new PostDto() { Content = Guid.NewGuid().ToString(), User = new UserInfoDto() };
            IdResultDto<long> expectedResult = new IdResultDto<long>() { IsOk = true, Message = Guid.NewGuid().ToString(), Id = new Random().Next(int.MaxValue) };
            _postServiceMock.Setup(x => x.AddRequestAsync(postDto, It.IsAny<WsClient>())).ReturnsAsync(expectedResult);
            _cacheServiceMock.Setup(x => x.GetWsClientsAsync()).ReturnsAsync(expectedWsClients);

            // Act
            IActionResult actionResult = await _controller.AddPost(postDto);

            // Assert
            actionResult.Should().NotBeNull().And.BeOfType<OkObjectResult>();

            _cacheServiceMock.Verify(x => x.GetWsClientsAsync(), Times.Once);
            _postServiceMock.Verify(x => x.AddRequestAsync(postDto, It.IsAny<WsClient>()), Times.Once);
            _loggerMock.Verify(m => m.Log(
                LogLevel.Debug,
                0,
                It.Is<FormattedLogValues>(v => v.ToString()
                .Contains($"Ok for: '{typeof(PostDto)}' -> Id: '{expectedResult.Id}'")),
                null,
                It.IsAny<Func<object, Exception, string>>()
                ));

            var objectResult = actionResult as OkObjectResult;
            objectResult.StatusCode.Should().Be(StatusCodes.Status200OK);
            objectResult.Value.Should().NotBeNull().And.BeOfType(typeof(IdResultDto<long>));
            var idResultDto = objectResult.Value as IdResultDto<long>;
            idResultDto.IsOk.Should().Be(true);
            idResultDto.Message.Should().NotBeNull().And.Be(expectedResult.Message);
            idResultDto.Id.Should().Be(expectedResult.Id);
        }
    }
}
