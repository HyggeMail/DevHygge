using System.Web.Http;
using System.Web.Http.Dispatcher;
using HyggeMail.App_Start;
using HyggeMail.Framework.Api.Logging;

namespace HyggeMail
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.MessageHandlers.Add(new ApiLogger());
        }
    }
}