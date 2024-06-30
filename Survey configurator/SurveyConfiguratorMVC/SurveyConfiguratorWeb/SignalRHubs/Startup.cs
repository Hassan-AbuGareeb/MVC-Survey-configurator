using Microsoft.Owin;
using Owin;
using SharedResources;
using System;

[assembly: OwinStartup(typeof(SurveyConfiguratorWeb.SignalRHubs.Startup))]

namespace SurveyConfiguratorWeb.SignalRHubs
{
    public class Startup
    {
        public static bool mIsDatabaseChangeEventRegistered = false;

        public void Configuration(IAppBuilder app)
        {
            try 
            { 
                app.MapSignalR();
            }
            catch (Exception ex)
            {
                UtilityMethods.LogError(ex);
            }
        }
    }
}
