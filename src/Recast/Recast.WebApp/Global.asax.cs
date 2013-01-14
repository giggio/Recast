using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Recast.WebApp
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            AzureStorage.CreateAllTables();
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AuthConfig.RegisterAuth();
            AutoMapping.MapAll();
            BootstrapBundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}