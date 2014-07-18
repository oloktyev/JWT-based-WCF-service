using System;
using System.IdentityModel.Protocols.WSTrust;
using System.IdentityModel.Tokens;
using EmbeddedAuthorizationServer.Models;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;

namespace EmbeddedAuthorizationServer.Controllers
{
    //[Authorize]
    public class IdentityController : ApiController
    {
        public string Get()
        {
            var securityKey = GetBytes("ThisIsAnImportantStringAndIHaveNoIdeaIfThisIsVerySecureOrNot!");

            var tokenHandler = new JwtSecurityTokenHandler();

            // Token Creation
            var now = DateTime.UtcNow;
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                         {
                             new Claim(ClaimTypes.Name, "John"),
                             new Claim(ClaimTypes.Role, "Author"),
                         }),
                TokenIssuerName = "self",
                AppliesToAddress = "http://www.example.com",
                Lifetime = new Lifetime(now, now.AddDays(7)),
                SigningCredentials = new SigningCredentials(
                    new InMemorySymmetricSecurityKey(securityKey),
                    "http://www.w3.org/2001/04/xmldsig-more#hmac-sha256",
                    "http://www.w3.org/2001/04/xmlenc#sha256"),
                TokenType = "urn:ietf:params:oauth:token-type:jwt"
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);
           return tokenString;

        }

        private byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;

        }

    }
}
