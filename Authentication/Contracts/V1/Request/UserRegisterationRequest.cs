using System.ComponentModel.DataAnnotations;

namespace AuthenticationService.Contracts.V1.Request
{
    public class UserRegisterationRequest
    {
        [EmailAddress]
        public string Email { get; set; }

        public string Password { get; set; }
    }
}
