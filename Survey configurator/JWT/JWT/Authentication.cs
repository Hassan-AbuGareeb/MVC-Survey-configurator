using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace JWT
{
    public class Authentication
    {
        public const string secret = "supserowaejroasrhfofasdfsadfawheroiawhrf";
        public const string issuer = "hassan";
        public const string audience = "users";
        public static string GenerateJWTAuth(string username, string password)
        {
            var claims = new[]
            {
                new Claim(JwtHeaderParameterNames.Jku, username),
                new Claim(JwtHeaderParameterNames.Kid, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Name, username)
            };

            var creds = new SigningCredentials(GetSecurityKey(), SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(1),
                signingCredentials : creds
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public static string ValidateToken(string token)
        {
            if (token == null)
            {
                return null;
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            JwtSecurityToken tokenObj = tokenHandler.ReadJwtToken(token);
            IEnumerable<Claim> claims = tokenObj.Claims;
            Debug.Write(claims.First(claim => claim.Type == ClaimTypes.Name).Value);
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = GetSecurityKey(),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);
                var jwtToken = (JwtSecurityToken)validatedToken;

                var jku = jwtToken.Claims.First(claim => claim.Type == "jku").Value;
                var userName = jwtToken.Claims.First(claim => claim.Type == "kid").Value;

                return userName;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static SymmetricSecurityKey GetSecurityKey()
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            return key;
        }


    }

}