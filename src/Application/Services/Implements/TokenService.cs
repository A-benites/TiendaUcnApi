using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using TiendaUcnApi.src.Application.Services.Interfaces;
using TiendaUcnApi.src.Domain.Models;

namespace TiendaUcnApi.src.Application.Services.Implements
{
    /// <summary>
    /// JWT token generation service implementation.
    /// </summary>
    public class TokenService : ITokenService
    {
        /// <summary>
        /// Application configuration.
        /// </summary>
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Secret key for JWT token signing.
        /// </summary>
        private readonly string _jwtSecret;

        /// <summary>
        /// Identity user manager.
        /// </summary>
        private readonly UserManager<User> _userManager;

        /// <summary>
        /// Initializes a new instance of the TokenService class.
        /// Injects configuration and UserManager dependencies.
        /// </summary>
        /// <param name="configuration">Application configuration.</param>
        /// <param name="userManager">User manager.</param>
        /// <exception cref="ArgumentNullException">Thrown when configuration is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown when JWT secret is not configured.</exception>
        public TokenService(IConfiguration configuration, UserManager<User> userManager)
        {
            _configuration =
                configuration ?? throw new ArgumentNullException(nameof(configuration));
            _jwtSecret =
                _configuration["JWTSecret"]
                ?? throw new InvalidOperationException("The JWT secret key is not configured.");
            _userManager = userManager;
        }

        /// <summary>
        /// Generates a JWT token for the user.
        /// Includes user ID, email, role, and security stamp in token claims.
        /// </summary>
        /// <param name="user">Authenticated user.</param>
        /// <param name="roleName">User's role.</param>
        /// <param name="rememberMe">Indicates if the token should last longer (24h vs 1h).</param>
        /// <returns>Generated JWT token string.</returns>
        /// <exception cref="InvalidOperationException">Thrown when token generation fails.</exception>
        public string GenerateToken(User user, string roleName, bool rememberMe = false)
        {
            try
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email!),
                    new Claim(ClaimTypes.Role, roleName),
                    new Claim(
                        new IdentityOptions().ClaimsIdentity.SecurityStampClaimType,
                        user.SecurityStamp!
                    ),
                };

                var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_jwtSecret));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    claims: claims,
                    expires: DateTime.Now.AddHours(rememberMe ? 24 : 1),
                    signingCredentials: creds
                );

                Log.Information("JWT token generated successfully for user {UserId}", user.Id);
                return new JwtSecurityTokenHandler().WriteToken(token);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error generating JWT token for user {UserId}", user.Id);
                throw new InvalidOperationException("Error generating JWT token", ex);
            }
        }
    }
}
