using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AuthService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly JwtSecurityTokenHandler _tokenHandler;

        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
            _tokenHandler = new JwtSecurityTokenHandler();
        }

        [HttpPost("token")]
        public IActionResult Token([FromBody] TokenRequest request)
        {
            // Validaciones del usuario y autenticación
            if (request.Username == "admin" && request.Password == "password")
            {
                var token = GenerateAccessToken(request.Username, "Admin");
                return Ok(new TokenResponse
                {
                    AccessToken = token,
                    ExpiresIn = 3600
                });
            }
            else if (request.Username == "cliente" && request.Password == "password")
            {
                var token = GenerateAccessToken(request.Username, "Cliente");
                return Ok(new TokenResponse
                {
                    AccessToken = token,
                    ExpiresIn = 3600
                });
            }

            return Unauthorized("Invalid credentials.");
        }

        private string GenerateAccessToken(string username, string role)
        {
            var privateKeyBase64 = _configuration["JWT:PrivateKey"];
            byte[] privateKeyBytes = Convert.FromBase64String(privateKeyBase64!);

            using var rsa = RSA.Create();
            rsa.ImportPkcs8PrivateKey(privateKeyBytes, out _); // Importar clave privada PKCS#8

            var rsaSecurityKey = new RsaSecurityKey(rsa);

            var credentials = new SigningCredentials(rsaSecurityKey, SecurityAlgorithms.RsaSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, username),
                new Claim(ClaimTypes.Role, role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:Issuer"],
                audience: _configuration["JWT:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(int.Parse(_configuration["JWT:TokenLifetimeMinutes"]!)),
                signingCredentials: credentials
            );

            return _tokenHandler.WriteToken(token);
        }
    }

    public class TokenRequest
    {
        public required string Username { get; set; }
        public required string Password { get; set; }
    }

    public class TokenResponse
    {
        public required string AccessToken { get; set; }
        public int ExpiresIn { get; set; }
    }
}
