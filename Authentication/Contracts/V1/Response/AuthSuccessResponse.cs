using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthenticationService.Contracts.V1.Response
{
    public class AuthSuccessResponse
    {
        public string JwtToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
