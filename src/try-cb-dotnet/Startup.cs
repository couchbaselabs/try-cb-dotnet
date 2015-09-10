using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(try_cb_dotnet.Startup))]
namespace try_cb_dotnet
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            //ConfigureAuth(app);
        }
    }
}
