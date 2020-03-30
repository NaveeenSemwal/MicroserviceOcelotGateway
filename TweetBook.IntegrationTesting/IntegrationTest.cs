using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using TweetBook.Contracts.V1;
using TweetBook.Contracts.V1.Request;
using TweetBook.Contracts.V1.Response;
using Xunit;

namespace TweetBook.IntegrationTesting
{
    /// <summary>
    /// 
    /// </summary>
    public class IntegrationTest : IClassFixture<CustomWebApplicationFactory<TweetBook.Startup>>
    {
        protected readonly HttpClient TestClient;

        private readonly CustomWebApplicationFactory<TweetBook.Startup> _factory;

        public IntegrationTest(CustomWebApplicationFactory<TweetBook.Startup> factory)
        {
            _factory = factory;
            TestClient= _factory.CreateClient();
        }


        protected async Task AuthenticateAsync()
        {
            TestClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", await JwtTokenAsync());
        }

        /// <summary>
        /// To get the token everytime you need to send different email/password
        /// </summary>
        /// <returns></returns>
        private async Task<string> JwtTokenAsync()
        {
            //string url = ApiRoutes.ControllerRoute + "/" + ApiRoutes.Identity.Register;

            //var response = await TestClient.PostAsJsonAsync(url, new UserRegisterationRequest
            //{
            //    Email = "champ1@gmail.com",
            //    Password = "Dotvik@9876"
            //});

            //var registerationResponse = await response.Content.ReadAsAsync<AuthSuccessResponse>();
            //return registerationResponse.JwtToken;

            return null;
        }
    }
}
