using AutoFixture;
using AutoMapper;
using FluentAssertions;
using Gugleus.Core.Domain;
using Gugleus.WebUI.Controllers;
using Gugleus.WebUI.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Gugleus.Tests.Erexus.Controllers
{
    public class RequestControllerTests
    {
        private readonly Fixture _fixture;
        private readonly Mock<IRequestSrv> _requestSrvMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<RequestsController>> _loggerMock;
        private readonly RequestsController _controller;

        public RequestControllerTests()
        {
            _fixture = new Fixture();
            _fixture.Customize(new RandomBooleanSequenceCustomization());

            _requestSrvMock = new Mock<IRequestSrv>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<RequestsController>>();
            _controller = new RequestsController( _mapperMock.Object, _requestSrvMock.Object,
                _loggerMock.Object);
        }

        [Theory(DisplayName = "Dev")]
        [InlineData(EnvType.Dev)]
        public async void Dev(EnvType env)
        {
            // Arrange

            // Act
            IActionResult actionResult = await _controller.List(env, null, null);

            // Assert
            actionResult.Should().NotBeNull().And.BeOfType<OkObjectResult>();
        }
    }
}
