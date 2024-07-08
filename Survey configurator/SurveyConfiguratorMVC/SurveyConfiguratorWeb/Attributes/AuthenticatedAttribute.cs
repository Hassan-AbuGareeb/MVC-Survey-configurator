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
    public class AuthenticatedAttribute : AuthorizeAttribute
    {
        public override void OnAuthorization(System.Web.Mvc.AuthorizationContext filterContext)
        {
            var tRequest = filterContext.HttpContext.Request;
            var tOriginalAccessToken = tRequest.Cookies[SharedConstants.cAccessTokenKey]?.Value;
            var tOriginalRefreshToken = tRequest.Cookies[SharedConstants.cRefreshTokenKey]?.Value;
            //has token
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
                    //check refresh token for validity
                    string tRefreshTokenId = TokenManager.GetTokenId(tOriginalRefreshToken);
                    OperationResult tTokenValidResult = AuthenticationServices.CheckTokenValidity(tRefreshTokenId);
                    if (!tTokenValidResult.IsSuccess)
                    {
                        //force delete cookies containing tokens 
                        filterContext.HttpContext.Response.Cookies[SharedConstants.cAccessTokenKey].Expires = DateTime.UtcNow.AddDays(SharedConstants.cCookiesForceDeletionTimeInDays);
                        filterContext.HttpContext.Response.Cookies[SharedConstants.cRefreshTokenKey].Expires = DateTime.UtcNow.AddDays(SharedConstants.cCookiesForceDeletionTimeInDays);

                        //redirect to log in page with error message
                        filterContext.Result = new RedirectToRouteResult(
                       new RouteValueDictionary(new { controller = SharedConstants.cLogInController, action = SharedConstants.cLogInIndexAction })
                       );
                        return;
                    }

                    //token not valid (expired)
                    //get claims form original access token
                    IEnumerable<Claim> tAccessTokenClaims = TokenManager.GetClaimsFromExpiredToken(tOriginalAccessToken);
                    string tNewAccessToken = TokenManager.RefreshJWT(tAccessTokenClaims, false);

                    //add tokens to cookies
                    HttpCookie tAccesstokenCookie = new HttpCookie(SharedConstants.cAccessTokenKey, tNewAccessToken);
                    filterContext.HttpContext.Response.Cookies.Add(tAccesstokenCookie);

                    //invalidate old refresh token
                    string tTokenId = TokenManager.GetTokenId(tOriginalRefreshToken);
                    AuthenticationServices.AddToken(tTokenId);

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
                //update user auth state
                States.IsAuthenticated = false;
                //add to viewData container a message to indicate that the user needs to be signed in to perform this action
                //no token redirect log in page
                filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary(new { controller = SharedConstants.cLogInController, action = SharedConstants.cLogInIndexAction })
                    );
            }
        }
    }
}