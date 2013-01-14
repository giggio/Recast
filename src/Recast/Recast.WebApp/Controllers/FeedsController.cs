using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using Microsoft.WindowsAzure.Storage.Table;
using Recast.WebApp.Infra;
using Recast.WebApp.Models.Entities;
using Recast.WebApp.Models.ViewModel;
using Recast.XmlFeed;

namespace Recast.WebApp.Controllers
{
    public class FeedsController : Controller
    {
        [HttpGet]
        public ActionResult New()
        {
            return View(new FeedViewModel());
        }

        [HttpPost]
        public ActionResult New(FeedViewModel feedViewModel)
        {
            if (!ModelState.IsValid) return View(feedViewModel);

            var account = AzureTableExtensions.GetStorageAccount();
            var client = account.CreateCloudTableClient();
            var table = client.GetTableReference("Feed");

            var feed = new Feed(feedViewModel.UserName, feedViewModel.FeedName);
            table.Insert(feed);

            return RedirectToRoute("ViewFeed", new { userName = feedViewModel.UserName, feedName = feedViewModel.FeedName});
        }  

        public ActionResult ViewFeed(string userName, string feedName)
        {
            ViewBag.UserName = userName;
            ViewBag.FeedName = feedName;
            var account = AzureTableExtensions.GetStorageAccount();
            var client = account.CreateCloudTableClient();
            var feedsTable = client.GetTableReference("Feed");
            var feed = (Feed)feedsTable.Execute(TableOperation.Retrieve<Feed>(userName, feedName)).Result;
            if (feed == null)
                return View();
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
            return View(new PostViewModel{UserName = userName, FeedName = feedName, PublishDate = DateTime.Now, SongLink = "http://"});
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

            return RedirectToRoute("ViewFeed", new { userName = post.GetUserName(), feedName = post.GetFeedName()});
        }
        [HttpGet]
        public ActionResult GoToFeed()
        {
            return View();
        }      
        [HttpPost]
        public ActionResult GoToFeed(FeedViewModel feedViewModel)
        {
            if (!ModelState.IsValid) return View(feedViewModel);
            return RedirectToRoute("ViewFeed", new { userName = feedViewModel.UserName, feedName = feedViewModel.FeedName });
        }

        public ActionResult GetFeed(string userName, string feedName)
        {
            var account = AzureTableExtensions.GetStorageAccount();
            var client = account.CreateCloudTableClient();
            var feedsTable = client.GetTableReference("Feed");
            var feed = (Feed)feedsTable.Execute(TableOperation.Retrieve<Feed>(userName, feedName)).Result;
            var query = new TableQuery<Post>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, Post.CreateKey(feed.PartitionKey, feed.RowKey)));
            var postsTable = client.GetTableReference("Post");
            var posts = postsTable.ExecuteQuery(query);
            var xml = FeedConverter.Create(new Uri(string.Format("{0}://{1}{2}", Request.Url.Scheme, Request.Url.Authority, UrlHelper.GenerateContentUrl("~", HttpContext))), feed, posts);
            return Content(xml, "text/xml");
        }

        public ActionResult DeletePost(string userName, string feedName, string title)
        {
            var account = AzureTableExtensions.GetStorageAccount();
            var client = account.CreateCloudTableClient();
            var postsTable = client.GetTableReference<Post>();
            var post = postsTable.Retrieve<Post>(Post.CreateKey(userName, feedName), HttpUtility.UrlEncode(title));
            if (post != null)
                postsTable.Delete(post);
            return RedirectToRoute("ViewFeed", new { userName, feedName });
        }

        [HttpGet]
        public ActionResult EditPost(string userName, string feedName, string title)
        {
            var account = AzureTableExtensions.GetStorageAccount();
            var client = account.CreateCloudTableClient();
            var postsTable = client.GetTableReference<Post>();
            var post = postsTable.Retrieve<Post>(Post.CreateKey(userName, feedName), HttpUtility.UrlEncode(title));
            if (post == null)
                return RedirectToRoute("ViewFeed", new { userName, feedName });
            var postViewModel = Mapper.Map<PostViewModel>(post);
            ViewBag.IsUpdate = true;
            return View(postViewModel);
        }

        [HttpPost]
        public ActionResult EditPost(PostViewModel postViewModel)
        {
            if (!ModelState.IsValid) return View(postViewModel);
            var account = AzureTableExtensions.GetStorageAccount();
            var client = account.CreateCloudTableClient();
            var postsTable = client.GetTableReference<Post>();
            var post = postsTable.Retrieve<Post>(Post.CreateKey(postViewModel.UserName, postViewModel.FeedName), HttpUtility.UrlEncode(postViewModel.Title));
            if (post == null)
                return RedirectToRoute("ViewFeed", new { userName = postViewModel.UserName, feedName = postViewModel.FeedName });
            Mapper.Map(postViewModel, post);
            postsTable.Merge(post);
            return RedirectToRoute("ViewFeed", new { userName = postViewModel.UserName, feedName = postViewModel.FeedName });
        }
    }
}
