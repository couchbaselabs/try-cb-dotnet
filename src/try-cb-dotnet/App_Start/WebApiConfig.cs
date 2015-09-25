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
                /// The API call's made from the Couchbase static HTML are making all call in the format:
                /// ControllerName/MethodName/parameters
                /// To support this {action} has been added to the routeTemplate for webAPI calls (call's beginning with api/....
                routeTemplate: "api/{controller}/{action}/{id}", /* added {action} name to uri*/
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
