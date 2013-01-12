using System.Collections.Generic;
using System.Web.Mvc;
using AutoMapper;
using Microsoft.WindowsAzure.Storage.Table;
using Recast.WebApp.Infra;
using Recast.WebApp.Models.Entities;
using Recast.WebApp.Models.ViewModel;

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

            var feed = new Feed(userName, name);
            table.Insert(feed);

            return RedirectToRoute("ViewFeed", new { userName, name });
        }  

        public ActionResult ViewFeed(string userName, string name)
        {
            var account = AzureTableExtensions.GetStorageAccount();
            var client = account.CreateCloudTableClient();
            var feedsTable = client.GetTableReference("Feed");
            var feed = (Feed)feedsTable.Execute(TableOperation.Retrieve<Feed>(userName, name)).Result;
            var query = new TableQuery<Post>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, Post.CreateKey(feed.PartitionKey, feed.RowKey)));
            var postsTable = client.GetTableReference("Post");
            var posts = postsTable.ExecuteQuery(query);
            var postsViewModels = Mapper.Map<IEnumerable<PostViewModel>>(posts);
            ViewBag.Posts = postsViewModels;
            return View(feed);
        }

        [HttpGet]
        public ActionResult AddPost(string userName, string feedName)
        {
            return View(new PostViewModel{UserName = userName, FeedName = feedName});
        }

        [HttpPost]
        public ActionResult AddPost(PostViewModel postView)
        {
            if (!ModelState.IsValid)
                return View(postView);
            var post = Mapper.Map<Post>(postView);

            var account = AzureTableExtensions.GetStorageAccount();
            var client = account.CreateCloudTableClient();
            var table = client.GetTableReference("Post");
            table.Insert(post);

            return RedirectToRoute("ViewFeed", new { userName = post.GetUserName(), name = post.GetFeedName()});
        }

        public ActionResult GoToFeed()
        {
            return View();
        }
    }
}
