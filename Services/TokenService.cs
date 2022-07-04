using Blog.Extensions;
using Blog.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Blog.Services
{
    public class TokenService
    {

        public string GenerateToken(User user)
        {

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(Configuration.JwtKey);
            var claims = user.GetClaims();

            var tokenDescriptior = new SecurityTokenDescriptor
            {

                /*
                Subject = new ClaimsIdentity(new Claim[] {
                    new Claim( ClaimTypes.Name, value: "Testado"), // user.Idendity.Name
                    new Claim( ClaimTypes.Role, value: "admin"), // user.IsInRole("")
                    new Claim( ClaimTypes.Role, value: "author"), 

                }), */
                
                Subject = new ClaimsIdentity(claims),

                Expires = DateTime.UtcNow.AddHours(12),
                SigningCredentials = new SigningCredentials(
                        new SymmetricSecurityKey(key),
                        SecurityAlgorithms.HmacSha256Signature
                    )
            };
            var token = tokenHandler.CreateToken(tokenDescriptior);
            return tokenHandler.WriteToken(token);

        }
    }
}
