using System.Web.Http;

namespace try_cb_dotnet
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            CouchbaseConfig.Register();
            GlobalConfiguration.Configure(WebApiConfig.Register);
        }

        protected void Application_End()
        {
            CouchbaseConfig.CleanUp();
        }
    }
}
