using InternalWork.Auth.Common.Models;
using InternalWork.Auth.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace InternalWork.Auth
{
    public static class Utils
    {
        public static string GenerateJwtToken(AppIdentityUser user, AuthSetting authSetting)
        {
            // generate token that is valid for 7 days
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(authSetting.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("id", user.Id.ToString()) }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public static Guid GetUserIdFromToken(ClaimsPrincipal User)
        {
            Claim userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                throw new UnauthorizedAccessException("Cannot find user id in token");
            }

            bool isParsed = Guid.TryParse(userIdClaim.Value, out Guid userId);
            if (!isParsed)
            {
                throw new UnauthorizedAccessException("Cannot find user id in token");
            }

            return userId;
        }
    }
}
