using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MyMovies.Startup))]
namespace MyMovies
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
