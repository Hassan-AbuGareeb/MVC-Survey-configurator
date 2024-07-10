using SharedResources;
using SurveyConfiguratorWeb.ConstantsAndMethods;
using SurveyConfiguratorWeb.Services;
using System;
using System.Net;
using System.Web.Mvc;

namespace SurveyConfiguratorWeb.Attributes
{
    public class AuthenticatedAPIAttribute : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            try { 
            //check headers for access token
            var tAccessToken = filterContext.HttpContext.Request.Headers[SharedConstants.cAccessTokenHeaderName];
                if (tAccessToken != null)
                {
                    //check token validity
                    bool tIsTokenValid = TokenManager.ValidateToken(tAccessToken);
                    if (tIsTokenValid)
                    {
                        //valid token proceed to resource
                        return;
                    }
                    //invalid token; expired, tempered with or incorrect token format
                    filterContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    filterContext.Result = new JsonResult { Data = new { Message = GlobalStrings.IncorrectToken }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                    return;
                }
                //no token
                filterContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                filterContext.Result = new JsonResult { Data = new { Message = GlobalStrings.TokenUnAuthorizedError }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                return;
            }
            catch(Exception ex)
            {
                UtilityMethods.LogError(ex);
                filterContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                filterContext.Result = new JsonResult { Data = new {Message = GlobalStrings.UnknownError}, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
        }
    }
}