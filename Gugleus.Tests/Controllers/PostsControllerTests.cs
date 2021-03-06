﻿using AutoFixture;
using AutoMapper;
using FluentAssertions;
using Gugleus.Api.Controllers;
using Gugleus.Core.Domain;
using Gugleus.Core.Domain.Dictionaries;
using Gugleus.Core.Dto.Input;
using Gugleus.Core.Dto.Output;
using Gugleus.Core.Results;
using Gugleus.Core.Services;
using Gugleus.GoogleCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Internal;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace Gugleus.Tests.Controllers
{
    public class PostsControllerTests
    {
        private readonly Fixture _fixture;
        private readonly Mock<IRequestService> _postServiceMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<PostsController>> _loggerMock;
        private readonly Mock<IActionContextAccessor> _httpActionContextMock;
        private readonly Mock<ICacheService> _cacheServiceMock;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly PostsController _controller;

        public PostsControllerTests()
        {
            _fixture = new Fixture();
            _fixture.Customize(new RandomBooleanSequenceCustomization());

            _postServiceMock = new Mock<IRequestService>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<PostsController>>();
            _httpActionContextMock = new Mock<IActionContextAccessor>();
            _cacheServiceMock = new Mock<ICacheService>();
            _configurationMock = new Mock<IConfiguration>();
            _controller = new PostsController(_postServiceMock.Object, _mapperMock.Object,
                _loggerMock.Object, _httpActionContextMock.Object, _cacheServiceMock.Object, _configurationMock.Object);

            // for mocking headers
            _controller.ControllerContext = new ControllerContext();
            _controller.ControllerContext.HttpContext = new DefaultHttpContext();
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
            long postId = _fixture.Create<long>();
            var requestType = RequestType.RequestTypeCode.ADDPOST;
            var expectedResponse = new ObjResult<RequestResponseDto<GoogleInfo>>() { IsOk = false };
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
                .Contains($"Request with Id: '{postId}' and type '{requestType}' not found")),
                null,
                It.IsAny<Func<object, Exception, string>>()
                ));

            actionResult.Should().NotBeNull().And.BeOfType<BadRequestObjectResult>();

            var badRequestResult = actionResult as BadRequestObjectResult;
            badRequestResult.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
            badRequestResult.Value.Should().NotBeNull().And.BeOfType<string>().And.Be($"Request with Id: '{postId}' not found...");
        }

        [Fact(DisplayName = "GetPostStatusInternalServerError")]
        public async void GetPostStatusInternalServerError()
        {
            // Arrange
            long postId = _fixture.Create<long>();
            var requestType = RequestType.RequestTypeCode.ADDPOST;
            Exception expectedException = _fixture.Create<Exception>();
            _postServiceMock
                .Setup(x => x.GetRequestResponseAsync<GoogleInfo>(postId, requestType))
                .Throws(expectedException);

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
                .Contains(
                    $"[GetRequestResponseAsync] Ex for Id: '{postId}' type: '{requestType}': System.Exception: {expectedException.Message}"
                    )),
                null,
                It.IsAny<Func<object, Exception, string>>()
                ));

            actionResult.Should().NotBeNull().And.BeOfType<ObjectResult>();

            var objectResult = actionResult as ObjectResult;
            objectResult.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
            objectResult.Value.Should().NotBeNull().And.BeOfType<string>();
            var requestResponse = objectResult.Value as string;
            requestResponse.Should().Be(expectedException.Message);
        }

        [Fact(DisplayName = "GetPostStatusOk")]
        public async void GetPostStatusOk()
        {
            // Arrange
            long postId = _fixture.Create<long>();
            var requestType = RequestType.RequestTypeCode.ADDPOST;
            string expectedRequestStatus = _fixture.Create<string>();
            var expectedResponse = new ObjResult<RequestResponseDto<GoogleInfo>>()
            { IsOk = true, Object = new RequestResponseDto<GoogleInfo> { Id = postId, Status = expectedRequestStatus } };
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
                .Contains($"Null input for '{typeof(PostDto).Name}'")),
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
                .Contains($"ValidErr for '{postDto.GetType().Name}': {expectedErrorMsg}")),
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
            List<WsClient> expectedWsClients = _fixture.CreateMany<WsClient>(1).ToList();
            PostDto postDto = _fixture.Create<PostDto>();
            IdResultDto<long> expectedResult = _fixture.Build<IdResultDto<long>>()
                .With(x => x.IsOk, false).With(x => x.Id, 0)
                .Create();

            _postServiceMock.Setup(x => x.AddRequestAsync(postDto, It.IsAny<WsClient>())).ReturnsAsync(expectedResult);
            _cacheServiceMock.Setup(x => x.GetWsClientsAsync()).ReturnsAsync(expectedWsClients);
            _controller.ControllerContext.HttpContext.Request.Headers["Hash"] = _fixture.Create<string>();

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
                .Contains($"Error for: '{postDto.GetType().Name}' -> Message: '{expectedResult.Message}'")),
                null,
                It.IsAny<Func<object, Exception, string>>()
                ));

            var objectResult = actionResult as ObjectResult;
            objectResult.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
            objectResult.Value.Should().NotBeNull().And.BeOfType(typeof(IdResultDto<long>));
            var idResultDto = objectResult.Value as IdResultDto<long>;
            idResultDto.IsOk.Should().Be(false);
            idResultDto.Message.Should().NotBeNull().And.Be(expectedResult.Message);
            idResultDto.Id.Should().Be(0, because: "Post shouldn't be added");
        }

        [Fact(DisplayName = "AddPostOk")]
        public async void AddPostOk()
        {
            // Arrange
            List<WsClient> expectedWsClients = _fixture.CreateMany<WsClient>(1).ToList();
            PostDto postDto = _fixture.Create<PostDto>();
            IdResultDto<long> expectedResult = _fixture.Build<IdResultDto<long>>()
                .With(x => x.IsOk, true)
                .Create();
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
                .Contains($"Ok for: '{postDto.GetType().Name}' -> Id: '{expectedResult.Id}'")),
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
