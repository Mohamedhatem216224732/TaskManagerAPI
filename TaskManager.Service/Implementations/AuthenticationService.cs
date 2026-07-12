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
            var claims = GetClaims(user);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var now = DateTime.UtcNow;
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
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var accessTokenString = tokenHandler.WriteToken(token);

            var refreshToken = GetRefreshToken(user.UserName);
            var userRefreshToken = new UserRefreshToken
            {
                AddedTime = DateTime.UtcNow,
                ExpiryDate = refreshToken?.ExpireAt ?? DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpireDate),
                IsRevoked = false,
                IsUsed = false,
                JwtId = token.Id,
                RefershToken = refreshToken?.TokenString ?? string.Empty,
                Token = accessTokenString,
                UserId = user.Id

            };
            var RefreshToken = await _refreshTokenRepository.AddAsync(userRefreshToken);
            if (RefreshToken == null)
            {
                throw new Exception("Failed to save refresh token.");
            }

            return new JwtAuthResult
            {
                AccessToken = accessTokenString,
                RefreshToken = refreshToken
            };
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
        };

            return claims;
        }




        #endregion
    }
}





