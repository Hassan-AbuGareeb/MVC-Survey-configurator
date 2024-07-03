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
            base.OnAuthorization(filterContext);
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