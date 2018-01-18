using FluentAssertions;
using Gugleus.Core.Dto.Input;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;
using System.Dynamic;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Gugleus.Tests.E2E
{
    public class PostE2eTests : BaseE2eTests
    {
        private readonly string _requestUri;

        public PostE2eTests()
        {
            _requestUri = "posts";
        }

        [Fact(DisplayName = "E2E_PingNoHash")]
        public async Task E2E_PingNoHash()
        {
            // Arrange

            // Act
            var response = await Client.GetAsync(_requestUri);

            // Assert
            response.StatusCode.ShouldBeEquivalentTo(StatusCodes.Status401Unauthorized);
            string responseString = await response.Content.ReadAsStringAsync();
            responseString.Should().Be(string.Empty);
        }

        [Fact(DisplayName = "E2E_PingBadHash")]
        public async Task E2E_PingBadHash()
        {
            // Arrange
            Client.DefaultRequestHeaders.Add("Hash", "123");

            // Act
            var response = await Client.GetAsync(_requestUri);

            // Assert
            response.StatusCode.ShouldBeEquivalentTo(StatusCodes.Status401Unauthorized);
            string responseString = await response.Content.ReadAsStringAsync();
            responseString.Should().Be(string.Empty);
        }

        [Fact(DisplayName = "E2E_PingOk")]
        public async Task E2E_PingOk()
        {
            // Arrange
            Client.DefaultRequestHeaders.Add("Hash", "abc");

            // Act
            var response = await Client.GetAsync(_requestUri);

            // Assert
            response.StatusCode.ShouldBeEquivalentTo(StatusCodes.Status200OK);
            string responseString = await response.Content.ReadAsStringAsync();
            responseString.Should().StartWith("Ping at ");
        }

        [Fact(DisplayName = "E2E_AddPostNotValid")]
        public async Task E2E_AddPostNotValid()
        {
            // Arrange
            Client.DefaultRequestHeaders.Add("Hash", "abc");
            PostDto post = new PostDto();
            StringContent payload = PrepareContent(post);

            // Act
            var response = await Client.PostAsync(_requestUri, payload);

            // Assert
            response.StatusCode.ShouldBeEquivalentTo(StatusCodes.Status400BadRequest);
            string responseString = await response.Content.ReadAsStringAsync();
            responseString.Should().NotBeNull();

            ExpandoObject modelStateErrors = JsonConvert.DeserializeObject<ExpandoObject>(responseString, new ExpandoObjectConverter());

            IDictionary<string, object> propertyValues = modelStateErrors;
            propertyValues.Should().NotBeNull().And.HaveCount(2);

            propertyValues["User"].Should().NotBeNull().And.BeOfType<List<object>>();
            var userProperty = propertyValues["User"] as List<object>;
            userProperty.Should().HaveCount(1);
            userProperty[0].Should().BeOfType<string>().And.Be("The User field is required.");

            propertyValues["Content"].Should().NotBeNull().And.BeOfType<List<object>>();
            var contentProperty = propertyValues["Content"] as List<object>;
            contentProperty.Should().HaveCount(1);
            contentProperty[0].Should().BeOfType<string>().And.Be("The Content field is required.");
        }

    }
}
