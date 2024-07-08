using Microsoft.IdentityModel.Tokens;
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
        /// <summary>
        /// this class is responsible for creating and validating
        /// JWTs, and all operations related to them
        /// </summary>

        //constants
        private const string cSecretKeySettingsKey = "Secret";
        private const string cIssuerSettingKey = "Issuer";
        private const string cAudienceSettingKey = "Audience";

        /// <summary>
        /// creates a new JWT from the received data
        /// </summary>
        /// <param name="pUserName">user name</param>
        /// <param name="pIsRefreshToken">indicates whether the requested token is and access or refresh token</param>
        /// <returns>string representing the newly created JWT</returns>
        public static string GenerateJWT(string pUserName, bool pIsRefreshToken)
        {
            try { 
                Claim[] tClaims = new[]
                {
                    new Claim(JwtHeaderParameterNames.Jku, pUserName),
                    new Claim(JwtHeaderParameterNames.Kid, Guid.NewGuid().ToString()),
                    new Claim(ClaimTypes.Name, pUserName),
                };
                return RefreshJWT(tClaims, pIsRefreshToken);
            }
            catch(Exception ex)
            {
                UtilityMethods.LogError(ex);
                return null;
            }
        }

        /// <summary>
        /// constructs a JWT from the received claims
        /// </summary>
        /// <param name="pClaims">claims to add to the token</param>
        /// <param name="pIsRefreshToken">indicates whether the requested token is and access or refresh token</param>
        /// <returns>string representing the newly created JWT</returns>
        public static string RefreshJWT(IEnumerable<Claim> pClaims, bool pIsRefreshToken)
        {
            try 
            { 
                //create signing creddintials from secret key
                SigningCredentials tCredintials = new SigningCredentials(GetSecurityKey(), SecurityAlgorithms.HmacSha256);

                //set the expire time based on whether it is a refresh token or not
                DateTime tExpiresIn;
                if (pIsRefreshToken)
                {
                    tExpiresIn = DateTime.UtcNow.AddDays(SharedData.cRefreshTokenExpireTimeInDays);
                }
                else
                {
                    tExpiresIn = DateTime.UtcNow.AddMinutes(SharedData.cAccessTokenExpireTimeInMinutes);
                }

                //set issure and audience
                string tIssuer = WebConfigurationManager.AppSettings[cIssuerSettingKey];
                string tAudience = WebConfigurationManager.AppSettings[cAudienceSettingKey];

                //create token
                JwtSecurityToken tToken = new JwtSecurityToken(
                    issuer: tIssuer,
                    audience: tAudience,
                    claims: pClaims,
                    expires: tExpiresIn,
                    signingCredentials: tCredintials
                    );

                return new JwtSecurityTokenHandler().WriteToken(tToken);
            }
            catch(Exception ex)
            {
                UtilityMethods.LogError(ex);
                return null;
            }
        }

        /// <summary>
        /// checks whether the received token is valid or not,
        /// checks for the tokens expiry, signing key,
        /// issuer and audience.
        /// </summary>
        /// <param name="pToken">token to validate</param>
        /// <returns>true if the token is valid</returns>
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
                    ValidateIssuer = false,
                    ValidateAudience = false,
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

        /// <summary>
        /// extracts the claims from the expired JWT
        /// </summary>
        /// <param name="pToken">expired token to extract claims from</param>
        /// <returns>claims of the expired token</returns>
        public static IEnumerable<Claim> GetClaimsFromExpiredToken(string pToken)
        {
            try 
            { 
                JwtSecurityTokenHandler tTokenHandler = new JwtSecurityTokenHandler();
                JwtSecurityToken tExpiredToken = tTokenHandler.ReadJwtToken(pToken);
                //iterate over the claims of the expired token
                //and add them to the list of claims
                List<Claim> tClaims = new List<Claim>();
                foreach (Claim tClaim in tExpiredToken.Claims)
                {
                    tClaims.Add(tClaim);
                }
                return tClaims;
            }
            catch(Exception ex)
            {
                UtilityMethods.LogError(ex);
                return null;
            }
        }

        /// <summary>
        /// extracts the token id from the received
        /// token through its claims
        /// </summary>
        /// <param name="pToken"> JWT token</param>
        /// <returns>string representing the token Id</returns>
        public static string GetTokenId(string pToken)
        {
            try
            {
                if (!string.IsNullOrEmpty(pToken))
                {
                    JwtSecurityTokenHandler tTokenHandler = new JwtSecurityTokenHandler();
                    JwtSecurityToken tToken = tTokenHandler.ReadJwtToken(pToken);

                    string tTokenId = "";
                    foreach (Claim tClaim in tToken.Claims)
                    {
                        if (tClaim.Type == JwtHeaderParameterNames.Kid)
                        {
                            tTokenId = tClaim.Value;
                        }
                    }
                    return tTokenId;
                }
                return null;
            }
            catch (Exception ex)
            {
                UtilityMethods.LogError(ex); 
                return null;
            }
        }

        #region utility functions
        /// <summary>
        /// creates a symmetric key using the secret key stored in the 
        /// web config file
        /// </summary>
        /// <returns>SymmetricSecurityKey used for signing the token</returns>
        private static SymmetricSecurityKey GetSecurityKey() 
        {
            try 
            { 
                string cSecretKey = WebConfigurationManager.AppSettings[cSecretKeySettingsKey];
                SymmetricSecurityKey tSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(cSecretKey));
                return tSigningKey;
            }
            catch (Exception ex) 
            {
                UtilityMethods.LogError(ex);
                return null;
            }
        }

        #endregion
    }
}