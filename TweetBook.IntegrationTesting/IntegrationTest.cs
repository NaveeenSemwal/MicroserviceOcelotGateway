using AuthenticationService.Contracts.V1;
using AuthenticationService.Contracts.V1.Request;
using AuthenticationService.Contracts.V1.Response;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Xunit;

namespace TweetBook.IntegrationTesting
{
    /// <summary>
    /// https://fullstackmark.com/post/20/painless-integration-testing-with-aspnet-core-web-api
    /// 
    /// https://docs.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-3.1
    /// </summary>
    //  public class IntegrationTest : IClassFixture<CustomWebApplicationFactory<AuthenticationService.Startup>>
    public class IntegrationTest
    {
        protected readonly HttpClient _authClient;
        readonly CustomWebApplicationFactory<AuthenticationService.Startup> factory;

        public IntegrationTest()
        {
            factory = new CustomWebApplicationFactory<AuthenticationService.Startup>();
            _authClient = factory.CreateClient();
        }

        protected async Task AuthenticateAsync()
        {
            _authClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", await JwtTokenAsync());
        }

        /// <summary>
        /// To get the token everytime you need to send different email/password
        /// </summary>
        /// <returns></returns>
        private async Task<string> JwtTokenAsync()
        {
            string url = ApiRoutes.ControllerRoute + "/" + ApiRoutes.Identity.Register;

            var response = await _authClient.PostAsJsonAsync(url, new UserRegisterationRequest
            {
                Email = "champ1@gmail.com",
                Password = "Dotvik@9876"
            });

            var registerationResponse = await response.Content.ReadAsAsync<AuthSuccessResponse>();
            return registerationResponse.JwtToken;
        }
    }
}
