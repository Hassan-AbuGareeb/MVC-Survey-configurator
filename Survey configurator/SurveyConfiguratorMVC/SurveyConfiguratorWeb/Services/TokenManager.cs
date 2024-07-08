﻿using Microsoft.IdentityModel.Tokens;
using SharedResources;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Web.Configuration;

namespace SurveyConfiguratorWeb.Services
{
    public class TokenManager
    {
        //constants

        private const string cSecretKeySettingsKey = "Secret";
        private const string cIssuerSettingKey = "Issuer";
        private const string cAudienceSettingKey = "Audience";

        public static string GenerateJWT(string pUserName, bool pIsRefreshToken)
        {
            Claim[] tClaims = new[]
            {
                new Claim(JwtHeaderParameterNames.Jku, pUserName),
                new Claim(JwtHeaderParameterNames.Kid, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Name, pUserName),
            };
            return RefreshJWT(tClaims, pIsRefreshToken);
        }

        public static string RefreshJWT(IEnumerable<Claim> pClaims, bool pIsRefreshToken)
        {
            
            SigningCredentials tCredintials = new SigningCredentials(GetSecurityKey(), SecurityAlgorithms.HmacSha256);

            DateTime tExpiresIn;
            if (pIsRefreshToken)
            {
                tExpiresIn = DateTime.UtcNow.AddDays(SharedData.cRefreshTokenExpireTimeInDays);
            }
            else
            {
                tExpiresIn = DateTime.UtcNow.AddMinutes(SharedData.cAccessTokenExpireTimeInMinutes);
            }

            string tIssuer = WebConfigurationManager.AppSettings[cIssuerSettingKey];
            string tAudience = WebConfigurationManager.AppSettings[cAudienceSettingKey];

            JwtSecurityToken tToken = new JwtSecurityToken(
                issuer: tIssuer,
                audience: tAudience,
                claims: pClaims,
                expires: tExpiresIn,
                signingCredentials: tCredintials
                );

            return new JwtSecurityTokenHandler().WriteToken(tToken);
        }

        public static bool ValidateToken(string pToken)
        {
            try
            {
                if (pToken == null)
                {
                    return false;
                }

                JwtSecurityTokenHandler tTokenHandler = new JwtSecurityTokenHandler();

                tTokenHandler.ValidateToken(pToken, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = GetSecurityKey(),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                return true;
            }
            catch (Exception ex)
            {
                UtilityMethods.LogError(ex);
                return false;
            }
        }

        public static IEnumerable<Claim> GetClaimsFromExpiredToken(string pToken)
        {
            JwtSecurityTokenHandler tTokenHandler = new JwtSecurityTokenHandler();
            JwtSecurityToken tExpiredToken = tTokenHandler.ReadJwtToken(pToken);

            List<Claim> tClaims = new List<Claim>();
            foreach (Claim tClaim in tExpiredToken.Claims)
            {
                tClaims.Add(tClaim);
            }
            return tClaims;
        } 

        public static string GetTokenId(string pToken)
        {
            if (!string.IsNullOrEmpty(pToken))
            {
                JwtSecurityTokenHandler tTokenHandler = new JwtSecurityTokenHandler();
                JwtSecurityToken tToken = tTokenHandler.ReadJwtToken(pToken);

                string tTokenId="";
                foreach (Claim tClaim in tToken.Claims)
                {
                    if(tClaim.Type == JwtHeaderParameterNames.Kid)
                    {
                        tTokenId = tClaim.Value;
                    }
                }
                return tTokenId;
            }
            return null;
        }

        #region utility functions
        private static SymmetricSecurityKey GetSecurityKey() 
        {
            string cSecretKey = WebConfigurationManager.AppSettings[cSecretKeySettingsKey];
            SymmetricSecurityKey tSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(cSecretKey));
            return tSigningKey;
        }

        #endregion
    }
}