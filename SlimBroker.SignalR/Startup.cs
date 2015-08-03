using System;
using Microsoft.Owin.Cors;
using Owin;

namespace SlimBroker.SignalR
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseCors(CorsOptions.AllowAll);
            app.MapSignalR();
        }
    }
}
