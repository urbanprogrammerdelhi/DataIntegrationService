using DataIntegrationService.Helpers;
using System.Web.Http;
using System.Web.Http.Cors;

namespace DataIntegrationService
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DataIntegrationApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.Filters.Add(new UnhandledExceptionFilter());
            var cors = new EnableCorsAttribute(
                            origins: "*",
                            headers: "*",
                            methods: "*");
            config.EnableCors(cors);
            config.Filters.Add(new AuthenticationFilterAttribute());
        }
    }
}
