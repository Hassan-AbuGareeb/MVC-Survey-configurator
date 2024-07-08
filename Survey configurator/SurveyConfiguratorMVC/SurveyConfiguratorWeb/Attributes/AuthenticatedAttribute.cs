using Azure;
using QuestionServices;
using SharedResources;
using SurveyConfiguratorWeb.ConstantsAndMethods;
using SurveyConfiguratorWeb.Controllers;
using SurveyConfiguratorWeb.Services;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace SurveyConfiguratorWeb.Attributes
{
    /// <summary>
    /// this attribute is responsible for enforcing the implemented
    /// Authentication method wherever it's called
    /// </summary>

    public class AuthenticatedAttribute : AuthorizeAttribute
    {
        /// <summary>
        /// main function of this filter which authenticates 
        /// whenever needed, it extracts the access and refresh
        /// tokens from the cookies and validates them, if the 
        /// access token is still valid (not expired) then proceed
        /// with the request,
        /// otherwise check the validity of the 
        /// refresh token, if it's still valid then issue new access 
        /// and refresh tokens and send them to the user and proceed to 
        /// the requested resource.
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnAuthorization(System.Web.Mvc.AuthorizationContext filterContext)
        {
            try
            { 
                //extract tokens from cookies
                var tRequest = filterContext.HttpContext.Request;
                var tOriginalAccessToken = tRequest.Cookies[SharedConstants.cAccessTokenKey]?.Value;
                var tOriginalRefreshToken = tRequest.Cookies[SharedConstants.cRefreshTokenKey]?.Value;
                //access token exists
                if (tOriginalAccessToken != null)
                {
                    //check token for validity
                    var isTokenValid = TokenManager.ValidateToken(tOriginalAccessToken);
                    if (isTokenValid)
                    {
                        //update user auth state
                        States.IsAuthenticated = true;
                        return;
                    }
                    else
                    {
                        //access token not valid (expired)
                        //check refresh token for validity
                        string tRefreshTokenId = TokenManager.GetTokenId(tOriginalRefreshToken);
                        OperationResult tTokenValidResult = AuthenticationServices.CheckTokenValidity(tRefreshTokenId);
                        //refresh token invalid
                        if (!tTokenValidResult.IsSuccess)
                        {
                            //force delete cookies containing tokens 
                            filterContext.HttpContext.Response.Cookies[SharedConstants.cAccessTokenKey].Expires = DateTime.UtcNow.AddDays(SharedConstants.cCookiesForceDeletionTimeInDays);
                            filterContext.HttpContext.Response.Cookies[SharedConstants.cRefreshTokenKey].Expires = DateTime.UtcNow.AddDays(SharedConstants.cCookiesForceDeletionTimeInDays);

                            //set user Auth state
                            States.IsAuthenticated = false;

                            //redirect to log in page with error message
                            filterContext.Result = new RedirectToRouteResult(
                           new RouteValueDictionary(new { controller = SharedConstants.cLogInController, action = SharedConstants.cLogInIndexAction })
                           );
                            return;
                        }

                        //invalidate old refresh token
                        string tTokenId = TokenManager.GetTokenId(tOriginalRefreshToken);
                        AuthenticationServices.AddToken(tTokenId);

                        //get claims form original access token
                        IEnumerable<Claim> tAccessTokenClaims = TokenManager.GetClaimsFromExpiredToken(tOriginalAccessToken);
                        string tNewAccessToken = TokenManager.RefreshJWT(tAccessTokenClaims, false);

                        //add token to cookies
                        HttpCookie tAccesstokenCookie = new HttpCookie(SharedConstants.cAccessTokenKey, tNewAccessToken);
                        filterContext.HttpContext.Response.Cookies.Add(tAccesstokenCookie);


                        //get claims form original refresh token
                        IEnumerable<Claim> tRefreshTokenClaims = TokenManager.GetClaimsFromExpiredToken(tOriginalRefreshToken);

                        //generate new refresh token
                        string tNewRefreshToken = TokenManager.RefreshJWT(tRefreshTokenClaims, true);

                        HttpCookie tRefreshtokenCookie = new HttpCookie(SharedConstants.cRefreshTokenKey, tNewRefreshToken)
                        {
                            HttpOnly = true
                        };
                        filterContext.HttpContext.Response.Cookies.Add(tRefreshtokenCookie);
                    
                        //update user auth state
                        States.IsAuthenticated = true;

                        //proceed to resource
                        return;
                    }
                }
                else
                {
                    //no access token 
                    //update user auth state
                    States.IsAuthenticated = false;
                    //no token redirect log in page
                    filterContext.Result = new RedirectToRouteResult(
                        new RouteValueDictionary(new { controller = SharedConstants.cLogInController, action = SharedConstants.cLogInIndexAction })
                        );
                }
            }
            catch ( Exception ex )
            {
                UtilityMethods.LogError(ex);
            }
        }
    }
}