using System.Web.Http;

namespace Recast.WebApp
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            ConfigureRoutes(config.Routes);

            //SetFormaterToXml(config);
        }

        private static void ConfigureRoutes(HttpRouteCollection routes)
        {
            routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }

        //private static void SetFormaterToXml(HttpConfiguration config)
        //{
        //    foreach (var mediaType in config.Formatters.FormUrlEncodedFormatter.SupportedMediaTypes)
        //    {
        //        config.Formatters.XmlFormatter.SupportedMediaTypes.Add(mediaType);
        //    }
        //    config.Formatters.Remove(config.Formatters.FormUrlEncodedFormatter);
        //    config.Formatters.Remove(config.Formatters.JsonFormatter);
        //    config.Formatters.XmlFormatter.UseXmlSerializer = true;
        //}
    }
}
