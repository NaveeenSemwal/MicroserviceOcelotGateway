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
        protected readonly HttpClient _postClient;
        readonly CustomWebApplicationFactory<TweetBook.Startup> factory;

        public PostControllerTest()
        {
            factory = new CustomWebApplicationFactory<TweetBook.Startup>();
            _postClient = factory.CreateClient();
        }

        [Fact]
        public async Task GetAll_WithoutAnyPosts_ReturnsEmptyRespose()
        {
            //Arrange
            await AuthenticateAsync();
            _postClient.DefaultRequestHeaders.Authorization = _authClient.DefaultRequestHeaders.Authorization;

            // Act
            string url = ApiRoutes.ControllerRoute + "/" + ApiRoutes.Posts.GetAll;
            var response = await _postClient.GetAsync(url);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var posts = await response.Content.ReadAsAsync<List<Post>>();
            (posts).Should().NotBeNullOrEmpty();
        }
    }
}
