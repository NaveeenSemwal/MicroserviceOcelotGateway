using FluentAssertions;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using TweetBook.Contracts.V1;
using TweetBook.Domain;
using Xunit;

namespace TweetBook.IntegrationTesting
{
    public class PostControllerTest : IntegrationTest
    {
        public PostControllerTest(CustomWebApplicationFactory<Startup> factory) : base(factory)
        {
        }

        [Fact]
        public async Task GetAll_WithoutAnyPosts_ReturnsEmptyRespose()
        {
            //Arrange
            await AuthenticateAsync();

            // Act
            string url = ApiRoutes.ControllerRoute + "/" + ApiRoutes.Posts.GetAll;
            var response = await TestClient.GetAsync(url);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            (await response.Content.ReadAsAsync<List<Post>>()).Should().BeEmpty();
        }
    }
}
