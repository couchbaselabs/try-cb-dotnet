using System.Threading;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(try_cb_dotnet.Startup))]

namespace try_cb_dotnet
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var context = new OwinContext(app.Properties);
            var token = context.Get<CancellationToken>("host.OnAppDisposing");
            if (token != CancellationToken.None)
            {
                token.Register(() =>
                {
                    // code to run on shutdown
                });
            }
        }
    }
}
