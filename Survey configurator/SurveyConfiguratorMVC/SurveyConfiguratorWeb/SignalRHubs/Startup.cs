using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(SurveyConfiguratorWeb.SignalRHubs.Startup))]

namespace SurveyConfiguratorWeb.SignalRHubs
{
    public class Startup
    {
        public static bool mIsEventRegistered = false;

        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
        }
    }
}
