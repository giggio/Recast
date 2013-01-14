using System.Web.Optimization;

namespace Recast.WebApp
{
	public class BootstrapBundleConfig
	{
		public static void RegisterBundles(BundleCollection bundles)
		{
			bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include("~/Content/bootstrap/js/bootstrap*"));
		}
	}
}
