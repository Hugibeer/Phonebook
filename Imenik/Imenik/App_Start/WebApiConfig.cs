using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Imenik
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services


            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute("Echo", "api/echo", new { controllers = "Echo" });

            config.Routes.MapHttpRoute(
                "Phone", "api/phones/{phonenumber}", new { controller = "Phone", phonenumber = RouteParameter.Optional }
                );

            config.Routes.MapHttpRoute(
                "Username", "api/contacts/{username}", new { controller = "Contact", username = RouteParameter.Optional }
                );
                        
            config.Routes.MapHttpRoute(
                "UserCRUD", "api/users/{username}", new { controller = "User", username = RouteParameter.Optional }
                );

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new {  id = RouteParameter.Optional }
            );
        }
    }
}
