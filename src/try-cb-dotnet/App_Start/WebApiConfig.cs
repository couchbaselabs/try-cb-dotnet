using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace try_cb_dotnet
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            config.Formatters.Remove(config.Formatters.XmlFormatter);
            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}/{id}", /* added {action} name to uri*/
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
