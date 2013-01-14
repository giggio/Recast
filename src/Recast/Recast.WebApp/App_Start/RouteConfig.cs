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
                name: "XMLFeed",
                url: "{userName}/{feedName}",
                defaults: new { controller = "Feeds", action = "GetFeed" },
                constraints: new { userName = @"^(?!feeds$|feed$|home$).*" }
            );

            routes.MapRoute(
                name: "ViewFeed",
                url: "Feeds/{userName}/{name}",
                defaults: new {controller = "Feeds", action = "ViewFeed"},
                constraints: new { userName = @"^(?!create$|new$).*", httpMethod = new HttpMethodConstraint("GET") }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}