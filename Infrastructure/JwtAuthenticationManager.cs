using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LaundryApi.Interfaces;
using LaundryApi.Dtos;
using LaundryApi.Entites;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace LaundryApi.Infrastructure
{
    public class JwtAuthenticationManager: IJwtAuthenticationManager
    {
        private string encryptionKey;
        public JwtAuthenticationManager(string encryptionKey)
        {
            this.encryptionKey = encryptionKey;
        }

        public string GetToken(UserLoginDto _user,string roleName)
        {
            //create security token handler
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenKey = Encoding.ASCII.GetBytes(encryptionKey);

            //create token descriptor
            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, _user.Username),
                    new Claim(ClaimTypes.Role, roleName)
                    
                }),
                Expires = DateTime.UtcNow.AddDays(60),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            
            return tokenHandler.WriteToken(token);

        }

        
    }
}
