using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace SurveyConfiguratorWeb
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Questions", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Errors",
                url: "Error/{action}/{ErrorMessage}",
                defaults: new { controller = "Error", action = "ErrorPage", ErrorMessage = "Page not found", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "wrongPath",
                url: "*any",
                defaults: new { controller = "Error", action = "ErrorPage", ErrorMessage = "Page not found", id = UrlParameter.Optional }
            );
        }
    }
}
