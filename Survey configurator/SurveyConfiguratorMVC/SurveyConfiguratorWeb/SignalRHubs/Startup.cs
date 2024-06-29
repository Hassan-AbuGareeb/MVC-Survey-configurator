using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(SurveyConfiguratorWeb.SignalRHubs.Startup))]

namespace SurveyConfiguratorWeb.SignalRHubs
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
        }
    }
}
