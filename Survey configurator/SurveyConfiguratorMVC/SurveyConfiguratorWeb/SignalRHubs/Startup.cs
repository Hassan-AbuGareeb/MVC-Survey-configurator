using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(SurveyConfiguratorWeb.SignalRHubs.Startup))]

namespace SurveyConfiguratorWeb.SignalRHubs
{
    public class Startup
    {
        public static bool mIsDatabaseChangeEventRegistered = false;

        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
        }
    }
}
