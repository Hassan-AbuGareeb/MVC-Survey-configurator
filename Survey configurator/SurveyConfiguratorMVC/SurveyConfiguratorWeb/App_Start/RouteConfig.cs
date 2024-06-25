using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;

namespace SurveyConfiguratorWeb
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            //ignore routes that tries to access this path
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            //main route for all existing controllers and their respective actions
            routes.MapRoute(
                name: "Default",
                url: "{Controller}/{action}/{id}",
                defaults: new { controller = "Questions", action = "Index", id = UrlParameter.Optional }
            );

            //route to handle the errors
            routes.MapRoute(
                name: "Errors",
                url: "Error/{action}/{ErrorMessage}",
                defaults: new { controller = "Error", action = "ErrorPage", ErrorMessage = "Page not found", id = UrlParameter.Optional }
            );

            // Catch-all route for 404 errors
            routes.MapRoute(
                name: "NotFound",
                url: "{*any}",
                defaults: new { controller = "Error", action = "ErrorPage", errorMessage = "404 Page not found" }
            );
        }
    }
}
