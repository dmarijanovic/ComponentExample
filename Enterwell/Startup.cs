using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Enterwell.Startup))]
namespace Enterwell
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }

    }
}
