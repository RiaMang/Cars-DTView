using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Cars_DTView.Startup))]
namespace Cars_DTView
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
