using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using TiendaUcnApi.src.Application.Services.Interfaces;
using TiendaUcnApi.src.Domain.Models;

namespace TiendaUcnApi.src.Application.Services.Implements
{
    /// <summary>
    /// Implementación del servicio de generación de tokens JWT.
    /// </summary>
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        private readonly string _jwtSecret;
        private readonly UserManager<User> _userManager; // Necesitas el UserManager

        // Modifica el constructor para inyectar UserManager
        public TokenService(IConfiguration configuration, UserManager<User> userManager)
        {
            _configuration =
                configuration ?? throw new ArgumentNullException(nameof(configuration));
            _jwtSecret =
                _configuration["JWTSecret"]
                ?? throw new InvalidOperationException("La clave secreta JWT no está configurada.");
            _userManager = userManager;
        }

        public string GenerateToken(User user, string roleName, bool rememberMe = false)
        {
            try
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email!),
                    new Claim(ClaimTypes.Role, roleName),
                    // --- LÍNEA CLAVE AÑADIDA ---
                    // Esta claim es la que buscará tu lógica de validación en Program.cs
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

                Log.Information(
                    "Token JWT generado exitosamente para el usuario {UserId}",
                    user.Id
                );
                return new JwtSecurityTokenHandler().WriteToken(token);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al generar el token JWT para el usuario {UserId}", user.Id);
                throw new InvalidOperationException("Error al generar el token JWT", ex);
            }
        }
    }
}