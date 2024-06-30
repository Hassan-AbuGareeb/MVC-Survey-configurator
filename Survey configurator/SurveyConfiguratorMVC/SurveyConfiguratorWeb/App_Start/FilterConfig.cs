using SharedResources;
using System;
using System.Web;
using System.Web.Mvc;

namespace SurveyConfiguratorWeb
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            try 
            { 
                filters.Add(new HandleErrorAttribute());
            }
            catch(Exception ex)
            {
                UtilityMethods.LogError(ex);
            }
        }
    }
}
