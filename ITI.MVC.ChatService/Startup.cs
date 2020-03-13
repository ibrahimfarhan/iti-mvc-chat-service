using ITI.MVC.ChatService.Models.Store;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ITI.MVC.ChatService.Startup))]
namespace ITI.MVC.ChatService
{
    public partial class Startup
    {
        private ApplicationDbContext dbCtx = ApplicationDbContext.Create();

        public ApplicationDbContext DbCtx
        {
            get => dbCtx ?? ApplicationDbContext.Create();

            private set => dbCtx = value;
        }

        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);

            app.MapSignalR();

            // TODO: Transfer this code to a function that runs right before program shutdown.
            // Remove all signalR client connections.
            DbCtx.ChatConnections.RemoveRange(DbCtx.ChatConnections);
            DbCtx.SaveChanges();
        }
    }
}
