using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Hypnofrog.Startup))]
namespace Hypnofrog
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
