using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace JWT.Filters
{
    public class JWTAuthAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        public JWTAuthAttribute()
        {

        }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            var request = filterContext.HttpContext.Request;
            var token = request.Cookies["token"]?.Value;

            var testCookie = request.Cookies["test"]?.Value;
            if (testCookie != null)
            {
                HttpCookie testCookies = new HttpCookie("test", "test cookie ultra pro max");
                filterContext.HttpContext.Response.Cookies.Add(testCookies);
            }
            else 
            { 
            HttpCookie testCookies = new HttpCookie("test", "test cookie");
            filterContext.HttpContext.Response.Cookies.Add(testCookies);
            }
            //if (token != null)
            //{
            //    var userName = Authentication.ValidateToken(token);
            //    if (userName != null)
            //    {
            //        return ;
            //    }
            //}
            //else
            //{
            //    return ;
            //}
            //return ;
        }

        //protected override bool AuthorizeCore(HttpContextBase httpContext)
        //{
        //    var request = httpContext.Request;
        //    var token = request.Cookies["token"]?.Value;

        //    if (token != null)
        //    {
        //        var userName = Authentication.ValidateToken(token);
        //        if (userName != null)
        //        {
        //            return true;
        //        }
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //    return true;
        //}
    }
}