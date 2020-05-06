using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

using AuthenticationService.Services.Abstract;
using AuthenticationService.Contracts.V1;
using AuthenticationService.Contracts.V1.Request;
using AuthenticationService.Contracts.V1.Response;
using TweetBook.Utilities;

namespace AuthenticationService.Controllers.V1
{
    [Route(ApiRoutes.ControllerRoute)]
    [ApiController]
    public class IdentityController : ControllerBase
    {
        private readonly IIdentityService _identityService;
        private readonly ILog _log;

        public IdentityController(IIdentityService identityService, ILog log)
        {
            _identityService = identityService;
            _log = log;
        }


        [HttpPost(ApiRoutes.Identity.Register)]
        public async Task<IActionResult> Register([FromBody] UserRegisterationRequest request)
        {
            _log.LogInfo("This is for Registeration service");
            var authResponse = await _identityService.RegisterAsync(request.Email, request.Password).ConfigureAwait(false);

           

            if (!authResponse.Success)
            {
                return BadRequest(new AuthFailResponse { Errors = authResponse.Errors });
            }

            return Ok(new AuthSuccessResponse { JwtToken = authResponse.JwtToken, RefreshToken= authResponse.RefreshToken });
        }

        [HttpPost(ApiRoutes.Identity.Login)]
        public async Task<IActionResult> Login([FromBody] UserLoginRequest request)
        {
            var authResponse = await _identityService.LoginAsync(request.Email, request.Password).ConfigureAwait(false);

            if (!authResponse.Success)
            {
                return BadRequest(new AuthFailResponse { Errors = authResponse.Errors });
            }

            return Ok(new AuthSuccessResponse { JwtToken = authResponse.JwtToken, RefreshToken = authResponse.RefreshToken });
        }


        [HttpPost(ApiRoutes.Identity.RefreshToken)]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            var authResponse = await _identityService.RefreshTokenAsync(request.JwtToken, request.RefreshToken).ConfigureAwait(false);

            if (!authResponse.Success)
            {
                return BadRequest(new AuthFailResponse { Errors = authResponse.Errors });
            }

            return Ok(new AuthSuccessResponse { JwtToken = authResponse.JwtToken, RefreshToken = authResponse.RefreshToken });
        }
    }
}