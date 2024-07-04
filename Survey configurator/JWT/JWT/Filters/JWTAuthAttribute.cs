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
            Authentication.ValidateToken(token);

        }
    }
}