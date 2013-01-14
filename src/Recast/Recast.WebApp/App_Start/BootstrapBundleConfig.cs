using System.Web.Optimization;

//[assembly: WebActivator.PostApplicationStartMethod(typeof(Recast.WebApp.BootstrapBundleConfig), "RegisterBundles")]

namespace Recast.WebApp
{
	public class BootstrapBundleConfig
	{
		public static void RegisterBundles(BundleCollection bundles)
		{
			// Add @Styles.Render("~/Content/bootstrap") in the <head/> of your _Layout.cshtml view
			// Add @Scripts.Render("~/bundles/bootstrap") after jQuery in your _Layout.cshtml view
			// When <compilation debug="true" />, MVC4 will render the full readable version. When set to <compilation debug="false" />, the minified version will be rendered automatically
			bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include("~/Content/bootstrap/js/bootstrap*"));
            bundles.Add(new StyleBundle("~/Content/bootstrap").Include("~/Content/bootstrap/css/bootstrap.css", "~/Content/bootstrap/css/bootstrap-responsive.css"));
		}
	}
}
