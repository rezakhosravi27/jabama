using DomainLayer.Dto;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ServiceLayer.IService;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace ServiceLayer.Service
{
    public class JwtService: IJwt
    {
        private readonly IConfiguration _configuration;
        private const int EXPIRATION_MINUTES = 6;
        public JwtService(IConfiguration configuration)
        {
            _configuration = configuration; 
        }
        public AuthenticateResponse CreateToken(IdentityUser user, List<Claim> claims)
        {
            var expiration = DateTime.UtcNow.AddMinutes(EXPIRATION_MINUTES);

            var credentials = new SigningCredentials(
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(_configuration["Jwt:Key"])
                ),
                SecurityAlgorithms.HmacSha256
            );

            var newToken = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: expiration,
                signingCredentials: credentials
            );

            var tokenHandler = new JwtSecurityTokenHandler();

            return new AuthenticateResponse
            {
                Token = tokenHandler.WriteToken(newToken),
                Expiration = expiration
            };
        }

    }
}
