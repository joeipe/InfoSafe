using FluentAssertions;
using InfoSafe.ViewModel;
using SharedKernel.Extensions;
using SharedKernel.Utils;
using System.Net;

namespace InfoSafe.IntegrationTests.Controllers
{
    public class ContactControllerShould : IClassFixture<ApiWebApplicationFactory>
    {
        protected readonly ApiWebApplicationFactory _factory;
        protected readonly HttpClient _client;

        public ContactControllerShould(ApiWebApplicationFactory fixture)
        {
            _factory = fixture;
            _client = _factory.CreateClient();
        }

        [Theory]
        [InlineData("/api/Contact/GetContactById/1")]
        public async Task GET_get_contact_by_id(string url)
        {
            // Arrange
            // Act
            var response = await _client.GetAsync(url);
            var data = await response.ReadContentAs<ContactVM>();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Theory]
        [InlineData("/api/Contact/SaveContact")]
        public async Task POST_add_contact(string url)
        {
            // Arrange
            var data = JsonFileReader.Read<ContactVM>(@"contact_add.json", @"IntegrationTestsData\");

            // Act
            var response = await _client.PostAsJson(url, data);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Theory]
        [InlineData("/api/Contact/SaveContact")]
        public async Task POST_edit_contact(string url)
        {
            // Arrange
            var data = JsonFileReader.Read<ContactVM>(@"contact_edit.json", @"IntegrationTestsData\");

            // Act
            var response = await _client.PostAsJson(url, data);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}