using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SotrageHub.Application;
using StorageHub.Infrastructure;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace StorageHub.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AuthenticateController : ControllerBase
    {
        private readonly JWTSettings _jwtSettings;
        private readonly LoginSettings _loginSettings;
        public AuthenticateController(IOptions<JWTSettings> jwtSettings, IOptions<LoginSettings> loginSettings)
        {
            _jwtSettings = jwtSettings.Value;
            _loginSettings = loginSettings.Value;
        }


        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDTO request)
        {
           
            if (request.Username == _loginSettings.Username && request.Password == _loginSettings.Password)
            {
                var token = GenerateJwtToken(request.Username);
                return Ok(new { token });
            }

            return Unauthorized("Invalid credentials");
        }

        private string GenerateJwtToken(string username)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey)); // same key from Program.cs
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, username),
                // you can add more claims here (role, email, etc.)
            };

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

}

