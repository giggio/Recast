using Microsoft.AspNetCore.Mvc;
using Recast.WebApp.Models;
using System.Diagnostics;

namespace Recast.WebApp.Controllers;

public class HomeController : Controller
{
    public IActionResult Index() => View();

    public IActionResult About()
    {
        ViewData["Message"] = "Your application description page.";

        return View();
    }

    public IActionResult Contact()
    {
        ViewData["Message"] = "Your contact page.";

        return View();
    }

    public IActionResult Privacy() => View();

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });

    public ActionResult HowToStart() => View();

    public ActionResult Support() => View();

    public ActionResult Contribute() => View();
}
