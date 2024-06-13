using System.Globalization;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace SurveyConfiguratorWeb
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            
            //set language for the app
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.GetCultureInfo("ar");
            CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.GetCultureInfo("ar");
        } 
    }
}
