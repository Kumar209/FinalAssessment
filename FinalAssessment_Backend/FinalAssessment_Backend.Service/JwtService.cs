using FinalAssessment_Backend.Models.Entities;
using FinalAssessment_Backend.ServiceInterface;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace FinalAssessment_Backend.Service
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _configuration;

        public JwtService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<string> GenerateToken(PrashantDbUser user)
        {
            var claims = new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"]),
                    new Claim(JwtRegisteredClaimNames.Jti , Guid.NewGuid().ToString()),
                    new Claim("Id", user.Id.ToString()),
                    new Claim("Email", user.Email.ToString())
                };

            //Converting minutes into int because configuration will always return string
            var expiryMinutes = int.Parse(_configuration["Jwt:ExpiryMinutes"]);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

            var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken
            (
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
                signingCredentials: signIn

            );

            //Writetoken used for serialize a JwtSecurityToken into a JWT using the compact serialization format
            string tokenValue = new JwtSecurityTokenHandler().WriteToken(token);

            return tokenValue;
        }


        // Validate a JWT token
        public async Task<int> ValidateJwtToken(string token)
        {
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);


            var tokenHandler = new JwtSecurityTokenHandler();

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                ValidIssuer = _configuration["Jwt:Issuer"],
                ValidAudience = _configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(key)
            };

            SecurityToken validatedToken;

            var principal = tokenHandler.ValidateToken(token, validationParameters, out validatedToken);


            // If the token is valid, return the user ID
            if (principal.Identity.IsAuthenticated)
            {
                var Id = int.Parse(principal.FindFirst("Id")?.Value ?? "0");
                return Id;
            }

            return -1;

        }
    }
}
