using System.Web.Mvc;
using System.Web.Routing;

namespace Recast.WebApp
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "ViewFeed",
                url: "Feed/{userName}/{name}",
                defaults: new {controller = "Feeds", action = "ViewFeed"},
                constraints: new { userName = @"^(?!create$|new$).*" }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}