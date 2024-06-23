using System.Globalization;
using System.Threading;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace SurveyConfiguratorWeb
{
    public class MvcApplication : System.Web.HttpApplication
    {
        //constants
        private const string cLanguageKey = "Language";

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            //get app language from app config and set it here
            string tAppLanguage = WebConfigurationManager.AppSettings[cLanguageKey];
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.GetCultureInfo(tAppLanguage);
            CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.GetCultureInfo(tAppLanguage);
        } 
    }
}
