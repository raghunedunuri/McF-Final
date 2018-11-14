using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(McF.Startup))]
namespace McF
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
