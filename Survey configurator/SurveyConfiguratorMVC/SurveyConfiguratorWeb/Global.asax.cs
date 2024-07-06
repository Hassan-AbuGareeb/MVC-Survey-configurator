using Microsoft.IdentityModel.Tokens;
using QuestionServices;
using SharedResources;
using SurveyConfiguratorWeb.ConstantsAndMethods;
using System;
using System.Globalization;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace SurveyConfiguratorWeb
{
    public class MvcApplication : System.Web.HttpApplication
    {
        /// <summary>
        /// this class is the enterance to the app and the first code that
        /// is reached when accessing the website, added some options here that
        /// are applicable to all parts of the app
        /// </summary>


        /// <summary>
        /// the first function that is called when accessing the website
        /// sets the configurations for the app
        /// </summary>
        protected void Application_Start()
        {
            try { 
                AreaRegistration.RegisterAllAreas();
                FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
                RouteConfig.RegisterRoutes(RouteTable.Routes);
                BundleConfig.RegisterBundles(BundleTable.Bundles);

                //get app language from app config and set it as default for any thread created
                string tAppLanguage = WebConfigurationManager.AppSettings[SharedConstants.cLagnaugeAppSettingKey];
                CultureInfo.DefaultThreadCurrentCulture = CultureInfo.GetCultureInfo(tAppLanguage);
                CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.GetCultureInfo(tAppLanguage);
                //set the state of the current app language
                States.CurrentAppLanguage = tAppLanguage;

                //get the connection string and set it in the database layer
                bool tCanGetConnectionString = QuestionOperations.GetConnectionString();
            }
            catch(Exception ex)
            {
                UtilityMethods.LogError(ex);
            }
        } 
    }
}
