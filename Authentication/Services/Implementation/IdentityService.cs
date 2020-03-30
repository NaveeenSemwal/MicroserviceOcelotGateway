using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using AuthenticationService.Data;
using AuthenticationService.Domain;
using AuthenticationService.Options;
using AuthenticationService.Services.Abstract;

namespace AuthenticationService.Services.Implementation
{
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _dbContext;
        private readonly JwtSettings _jwtSettings;
        private readonly TokenValidationParameters _validationParameters;

        public IdentityService(UserManager<IdentityUser> userManager, ApplicationDbContext dbContext,
            JwtSettings jwtSettings, TokenValidationParameters validationParameters)
        {
            _userManager = userManager;
            _dbContext = dbContext;
            _jwtSettings = jwtSettings;
            _validationParameters = validationParameters;
        }

        public async Task<AuthenticationResult> LoginAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email).ConfigureAwait(false);

            if (user == null)
            {
                return new AuthenticationResult
                {
                    Errors = new[] { "User doesnot exists." }
                };
            }

            var userHasValidPassword = await _userManager.CheckPasswordAsync(user, password).ConfigureAwait(false); ;

            if (!userHasValidPassword)
            {
                return new AuthenticationResult
                {
                    Errors = new[] { "Email/Password combination is not matching." }
                };
            }

            return await GenerateAuthenticationResultForUser(user).ConfigureAwait(false);
        }

        public async Task<AuthenticationResult> RefreshTokenAsync(string jwtToken, string refreshToken)
        {
            ClaimsPrincipal claimsPrincipal;
            JwtSecurityToken securityToken = ValidateAndDecode(jwtToken, out claimsPrincipal);

            var Jti = claimsPrincipal.Claims.SingleOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti).Value;

            if (securityToken == null)
            {
                return new AuthenticationResult
                {
                    Errors = new[] { "Invalid Token !" }
                };
            }

            var storedToken = _dbContext.RefreshTokens.AsNoTracking().SingleOrDefault(x => x.Token == refreshToken);

            if (storedToken == null)
            {
                return new AuthenticationResult
                {
                    Errors = new[] { "This refresh token does not exists !" }
                };
            }

            if (DateTime.UtcNow > storedToken.ExpiryDate)
            {
                return new AuthenticationResult
                {
                    Errors = new[] { "This refresh token has expired !" }
                };
            }

            if (storedToken.IsInvalided)
            {
                return new AuthenticationResult { Errors = new[] { "This refresh token has been invalidated" } };
            }

            if (storedToken.IsUsed)
            {
                return new AuthenticationResult { Errors = new[] { "This refresh token has been used" } };
            }

            if (storedToken.JwtId != Jti)
            {
                return new AuthenticationResult
                {
                    Errors = new[] { "This refresh token doesnot match with this JWT." }
                };
            }

            storedToken.IsUsed = true;
            _dbContext.RefreshTokens.Update(storedToken);
            _dbContext.SaveChanges();

            string userId = claimsPrincipal.Claims.SingleOrDefault(x => x.Type == "id").Value;
            var user = await _userManager.FindByIdAsync(userId).ConfigureAwait(false);

            return await GenerateAuthenticationResultForUser(user).ConfigureAwait(false);


        }

        private JwtSecurityToken ValidateAndDecode(string jwtToken, out ClaimsPrincipal claimsPrincipal)
        {
            try
            {
                claimsPrincipal = new JwtSecurityTokenHandler()
                   .ValidateToken(jwtToken, _validationParameters, out var rawValidatedToken);

                return (JwtSecurityToken)rawValidatedToken;
                // Or, you can return the ClaimsPrincipal
                // (which has the JWT properties automatically mapped to .NET claims)
            }
            catch (SecurityTokenValidationException stvex)
            {
                // The token failed validation!
                // TODO: Log it or display an error.
                throw new Exception($"Token failed validation: {stvex.Message}");
            }
            catch (ArgumentException argex)
            {
                // The token was not well-formed or was invalid for some other reason.
                // TODO: Log it or display an error.
                throw new Exception($"Token was invalid: {argex.Message}");
            }
        }

        /// <summary>
        /// Register new user and create Token for upcoming request.
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<AuthenticationResult> RegisterAsync(string email, string password)
        {
            var existingUser = await _userManager.FindByEmailAsync(email).ConfigureAwait(false);

            if (existingUser != null)
            {
                return new AuthenticationResult
                {
                    Errors = new[] { "User with this email address already exists." }
                };
            }

            var newUser = new IdentityUser { Email = email, UserName = email };

            var createdUser = await _userManager.CreateAsync(newUser, password).ConfigureAwait(false);

            if (!createdUser.Succeeded)
            {
                return new AuthenticationResult
                {
                    Errors = createdUser.Errors.Select(x => x.Description)
                };
            }

            return await GenerateAuthenticationResultForUser(newUser).ConfigureAwait(false);

        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        /// <summary>
        /// Yes, the client needs to store the expiry date in the local storage and on every request you need to check if it’s in the past.
        ///  If it is then you use a middleware to call the refresh endpoint and get a new set of keys.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        private async Task<AuthenticationResult> GenerateAuthenticationResultForUser(IdentityUser user)
        {
            // authentication successful so generate jwt token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti,  Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim("id", user.Id)

                }),
                Expires = DateTime.UtcNow.Add(_jwtSettings.TokenLifetime),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);

            // string refreshToken = ValidateRefreshToken(user);

            var refreshTokenModel = new RefreshToken
            {
                JwtId = token.Id,
                UserId = user.Id,
                CreatedDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddMonths(6)
            };

            await _dbContext.RefreshTokens.AddAsync(refreshTokenModel);
            await _dbContext.SaveChangesAsync().ConfigureAwait(false);


            return new AuthenticationResult
            {
                Success = true,
                JwtToken = tokenHandler.WriteToken(token),
                RefreshToken = refreshTokenModel.Token
            };
        }

        private string ValidateRefreshToken(IdentityUser user)
        {
            var existingRefreshToken = _dbContext.RefreshTokens.SingleOrDefault(x => x.UserId == user.Id);

            string refreshToken = string.Empty;

            if (existingRefreshToken == null)
            {
                refreshToken = GenerateRefreshToken();
            }
            else if (DateTime.UtcNow > existingRefreshToken.ExpiryDate)
            {
                refreshToken = GenerateRefreshToken();
            }
            else if (existingRefreshToken.IsInvalided)
            {
                refreshToken = GenerateRefreshToken();
            }
            else
            {
                refreshToken = existingRefreshToken.Token;
            }

            return refreshToken;
        }
    }
}
