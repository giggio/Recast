using System.Web.Mvc;

namespace Recast.WebApp.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult HowToStart()
        {
            return View();
        }

        public ActionResult Support()
        {
            return View();
        }

        public ActionResult Contribute()
        {
            return View();
        }
    }
}