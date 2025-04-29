using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SimpleBearerTokenApp.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SimpleBearerTokenApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("login")]
        [ProducesResponseType(typeof(TokenResponse), 200)]
        [ProducesResponseType(401)]
        public IActionResult Login([FromBody] LoginModel model)
        {
            // Validate credentials.
            if (model.Username != "test" || model.Password != "password")
            {
                return Unauthorized();
            }

            // Retrieve the secret key from configuration.
            var secretKey = _configuration["Jwt:Secret"];
            if (string.IsNullOrEmpty(secretKey))
            {
                throw new Exception("JWT secret key is not configured properly.");
            }

            var key = Encoding.ASCII.GetBytes(secretKey);

            // Create claims for the token.
            var claims = new[]
            {
            new Claim(ClaimTypes.Name, model.Username)
        };

            // Build security token descriptor.
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            // Create the token.
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            // Return a strong-typed response.
            return Ok(new TokenResponse { Token = tokenString });
        }
    }
}
