using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

using AuthenticationService.Services.Abstract;
using AuthenticationService.Contracts.V1;
using AuthenticationService.Contracts.V1.Request;
using AuthenticationService.Contracts.V1.Response;

namespace AuthenticationService.Controllers.V1
{
    [Route(ApiRoutes.ControllerRoute)]
    [ApiController]
    public class IdentityController : ControllerBase
    {
        private readonly IIdentityService _identityService;

        public IdentityController(IIdentityService identityService)
        {
            _identityService = identityService;
        }


        [HttpPost(ApiRoutes.Identity.Register)]
        public async Task<IActionResult> Register([FromBody] UserRegisterationRequest request)
        {
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