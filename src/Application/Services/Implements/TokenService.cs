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
        /// <summary>
        /// Configuración de la aplicación.
        /// </summary>
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Clave secreta para la firma de los tokens JWT.
        /// </summary>
        private readonly string _jwtSecret;

        /// <summary>
        /// Administrador de usuarios de Identity.
        /// </summary>
        private readonly UserManager<User> _userManager;

        /// <summary>
        /// Constructor que inyecta la configuración y el UserManager.
        /// </summary>
        /// <param name="configuration">Configuración de la aplicación.</param>
        /// <param name="userManager">Administrador de usuarios.</param>
        public TokenService(IConfiguration configuration, UserManager<User> userManager)
        {
            _configuration =
                configuration ?? throw new ArgumentNullException(nameof(configuration));
            _jwtSecret =
                _configuration["JWTSecret"]
                ?? throw new InvalidOperationException("La clave secreta JWT no está configurada.");
            _userManager = userManager;
        }

        /// <summary>
        /// Genera un token JWT para el usuario.
        /// </summary>
        /// <param name="user">Usuario autenticado.</param>
        /// <param name="roleName">Rol del usuario.</param>
        /// <param name="rememberMe">Indica si el token debe durar más tiempo.</param>
        /// <returns>Token JWT generado.</returns>
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