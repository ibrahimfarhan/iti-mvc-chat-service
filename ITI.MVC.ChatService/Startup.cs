using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ITI.MVC.ChatService.Startup))]
namespace ITI.MVC.ChatService
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
