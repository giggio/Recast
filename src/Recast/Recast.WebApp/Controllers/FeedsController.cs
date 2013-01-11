using System.Web.Mvc;
using Microsoft.WindowsAzure.Storage.Table;
using Recast.WebApp.Infra;
using Recast.WebApp.Models.Entities;

namespace Recast.WebApp.Controllers
{
    public class FeedsController : Controller
    {
        [HttpGet]
        public ActionResult New()
        {
            return View();
        }

        [HttpPost]
        public ActionResult New(string userName, string name)
        {
            var account = AzureTableExtensions.GetStorageAccount();
            var client = account.CreateCloudTableClient();
            var table = client.GetTableReference("Feed");
            table.CreateIfNotExists();

            var feed = new Feed(userName, name);
            table.Insert(feed);

            return RedirectToRoute("ViewFeed", new { userName, name });
        }  

        public ActionResult ViewFeed(string userName, string name)
        {
            var account = AzureTableExtensions.GetStorageAccount();
            var client = account.CreateCloudTableClient();
            var table = client.GetTableReference("Feed");
            var feed = (Feed)table.Execute(TableOperation.Retrieve<Feed>(userName, name)).Result;
            return View(feed);
        }



    }
}
