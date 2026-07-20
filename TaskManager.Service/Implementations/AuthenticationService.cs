using Microsoft.IdentityModel.Tokens;
using Project_Task_Management.Data.Entities.Identity;
using Project_Task_Management.Data.Helpers;
using System.Collections.Concurrent;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using TaskManager.Infrastructure.Abstracts;
using TaskManager.Service.Abstracts;

namespace TaskManager.Service.Implementations
{
    public class AuthenticationService : IAuthenticationService
    {
        #region Fields
        private readonly JwtSettings _jwtSettings;
        private readonly ConcurrentDictionary<string, RefreshToken> _refreshTokens;
        private readonly IRefershTokenRepository _refreshTokenRepository;
        #endregion

        #region Constructors
        public AuthenticationService(JwtSettings jwtSettings, IRefershTokenRepository refreshTokenRepository)
        {

            _jwtSettings = jwtSettings;
            _refreshTokens = new ConcurrentDictionary<string, RefreshToken>();
            _refreshTokenRepository = refreshTokenRepository;
        }
        #endregion

        #region Functions
        public async Task<JwtAuthResult> GetJWTToken(ApplicationUser user)
        {
            // Call the helper exactly once and deconstruct the tuple
            var (jwtToken, accessTokenString) = GenerateJWTTokenDescriptor(user);

            var refreshToken = GetRefreshToken(user.UserName);

            var userRefreshToken = new UserRefreshToken
            {
                AddedTime = DateTime.UtcNow,
                ExpiryDate = refreshToken?.ExpireAt ?? DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpireDate),
                IsRevoked = false,
                IsUsed = false, // Note: Set to false initially so it can be verified/used later
                JwtId = jwtToken.Id, // Accessible now because jwtToken is a valid JwtSecurityToken
                RefershToken = refreshToken?.TokenString ?? string.Empty,
                Token = accessTokenString,
                UserId = user.Id
            };

            var savedRefreshToken = await _refreshTokenRepository.AddAsync(userRefreshToken);
            if (savedRefreshToken == null)
            {
                throw new Exception("Failed to save refresh token.");
            }

            return new JwtAuthResult
            {
                AccessToken = accessTokenString,
                RefreshToken = refreshToken
            };
        }

        private async Task<(JwtSecurityToken, string)> GenerateJWTToken(ApplicationUser user)
        {
            var claims = GetClaims(user);
            var jwtToken = new JwtSecurityToken(
                _jwtSettings.Issuer,
                _jwtSettings.Audience,
                claims,
                expires: DateTime.Now.AddDays(_jwtSettings.AccessTokenExpireDate),
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_jwtSettings.Secret)), SecurityAlgorithms.HmacSha256Signature));
            var accessToken = new JwtSecurityTokenHandler().WriteToken(jwtToken);
            return (jwtToken, accessToken);
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

        private RefreshToken? GetRefreshToken(string username)
        {
            var refreshToken = new RefreshToken
            {
                UserName = username ?? string.Empty,
                TokenString = GenerateRefreshToken(),
                ExpireAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpireDate)
            };
            _refreshTokens.AddOrUpdate(refreshToken.TokenString, refreshToken, (key, oldValue) => refreshToken);

            return refreshToken;

        }

        public List<Claim> GetClaims(ApplicationUser user)
        {
            var claims = new List<Claim>
        {
            new Claim(nameof(UserClaimModel.Id), user.Id.ToString()),
            new Claim(nameof(UserClaimModel.UserName), user.UserName ?? string.Empty),
            new Claim(nameof(UserClaimModel.Email), user.Email ?? string.Empty),
            new Claim(nameof(UserClaimModel.PhoneNumber), user.PhoneNumber ?? string.Empty)
        };

            return claims;
        }


        private (JwtSecurityToken TokenObject, string TokenString) GenerateJWTTokenDescriptor(ApplicationUser user)
        {
            var claims = GetClaims(user);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var now = DateTime.UtcNow;

            if (!claims.Any(c => c.Type == JwtRegisteredClaimNames.Jti))
            {
                claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                NotBefore = now,
                Expires = now.AddMinutes(_jwtSettings.AccessTokenExpireDate),
                SigningCredentials = creds,
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(tokenDescriptor) as JwtSecurityToken;

            if (securityToken == null)
            {
                throw new Exception("Failed to create JwtSecurityToken instance.");
            }

            var accessTokenString = tokenHandler.WriteToken(securityToken);

            return (securityToken, accessTokenString);
        }

        public async Task<JwtAuthResult> GetRefreshToken(ApplicationUser user, JwtSecurityToken jwtToken, DateTime? expiryDate, string refreshToken)
        {
            var (jwtSecurityToken, newToken) = await GenerateJWTToken(user);
            var response = new JwtAuthResult();
            response.AccessToken = newToken;
            var refreshTokenResult = new RefreshToken();
            refreshTokenResult.UserName = jwtToken.Claims.FirstOrDefault(x => x.Type == nameof(UserClaimModel.UserName)).Value;
            refreshTokenResult.TokenString = refreshToken;
            refreshTokenResult.ExpireAt = (DateTime)expiryDate;
            // response.refreshToken = refreshTokenResult;
            return response;

        }
        public JwtSecurityToken ReadJWTToken(string accessToken)
        {
            if (string.IsNullOrEmpty(accessToken))
            {
                throw new ArgumentNullException(nameof(accessToken));
            }
            var handler = new JwtSecurityTokenHandler();
            var response = handler.ReadJwtToken(accessToken);
            return response;
        }

        public async Task<string> ValidateToken(string accessToken)
        {
            var handler = new JwtSecurityTokenHandler();
            var parameters = new TokenValidationParameters
            {
                ValidateIssuer = _jwtSettings.ValidateIssuer,
                ValidIssuers = new[] { _jwtSettings.Issuer },
                ValidateIssuerSigningKey = _jwtSettings.ValidateIssuerSigningKey,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_jwtSettings.Secret)),
                ValidAudience = _jwtSettings.Audience,
                ValidateAudience = _jwtSettings.ValidateAudience,
                //ValidateLifetime = _jwtSettings.ValidateLifeTime,
            };
            try
            {
                var validator = handler.ValidateToken(accessToken, parameters, out SecurityToken validatedToken);

                if (validator == null)
                {
                    return "InvalidToken";
                }

                return "NotExpired";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public Task<(string, DateTime?)> ValidateDetails(JwtSecurityToken jwtToken, string AccessToken, string RefreshToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> ConfirmEmail(int? userId, string? code)
        {
            throw new NotImplementedException();
        }

        public Task<string> SendResetPasswordCode(string Email)
        {
            throw new NotImplementedException();
        }

        public Task<string> ConfirmResetPassword(string Code, string Email)
        {
            throw new NotImplementedException();
        }

        public Task<string> ResetPassword(string Email, string Password)
        {
            throw new NotImplementedException();
        }



        #endregion
    }
}





