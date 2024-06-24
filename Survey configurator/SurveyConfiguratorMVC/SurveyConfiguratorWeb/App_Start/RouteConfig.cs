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
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            //default route for the existing controllers and their actions
            routes.MapRoute(
                name: "Default",
                url: "{Controller}/{action}/{id}",
                defaults: new { controller = "Questions", action = "Index", id = UrlParameter.Optional }
            );

            //route for handling errors
            routes.MapRoute(
                name: "Errors",
                url: "Error/{action}/{ErrorMessage}",
                defaults: new { controller = "Error", action = "ErrorPage", ErrorMessage = "Page not found"}
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
