using System;
using System.Collections.Generic;
using System.Web.Mvc;
using AutoMapper;
using Recast.WebApp.Models;
using Recast.WebApp.Models.Entities;
using Recast.WebApp.Models.ViewModel;
using Recast.XmlFeed;

namespace Recast.WebApp.Controllers
{
    public class FeedsController : Controller
    {
        private readonly Feeds feeds;
        private readonly Posts posts;

        public FeedsController()
        {
            feeds = new Feeds();
            posts = new Posts();
        }

        [HttpGet]
        public ActionResult New()
        {
            return View(new FeedViewModel());
        }

        [HttpPost]
        public ActionResult New(FeedViewModel feedViewModel)
        {
            if (!ModelState.IsValid) return View(feedViewModel);
            var feed = new Feed(feedViewModel.UserName, feedViewModel.FeedName);

            var created = feeds.Insert(feed);
            if (created)
            {
                return RedirectToRoute("ViewFeed", new { userName = feedViewModel.UserName, feedName = feedViewModel.FeedName });                
            }
            ViewBag.IsDuplicate = true;
            return View(feedViewModel);
        }  

        public ActionResult ViewFeed(string userName, string feedName)
        {
            ViewBag.UserName = userName;
            ViewBag.FeedName = feedName;
            var feed = feeds.Get(userName, feedName);
            if (feed == null)
                return View();
            var feedPosts = posts.GetForFeed(feed);
            ViewBag.Posts = Mapper.Map<IEnumerable<PostViewModel>>(feedPosts);
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
            if (!ModelState.IsValid) return View(postView);

            var post = Mapper.Map<Post>(postView);
            posts.Insert(post);
            
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
            var feed = feeds.Get(userName, feedName);
            var feedPosts = posts.GetForFeed(feed);
            var xml = FeedConverter.Create(new Uri(string.Format("{0}://{1}{2}", Request.Url.Scheme, Request.Url.Authority, UrlHelper.GenerateContentUrl("~", HttpContext))), feed, feedPosts);
            return Content(xml, "text/xml");
        }

        public ActionResult DeletePost(string userName, string feedName, string title)
        {
            posts.Delete(userName, feedName, title);
            return RedirectToRoute("ViewFeed", new { userName, feedName });
        }

        [HttpGet]
        public ActionResult EditPost(string userName, string feedName, string title)
        {
            var post = posts.Get(userName, feedName, title);
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
            var post = posts.Get(postViewModel.UserName, postViewModel.FeedName, postViewModel.Title);
            if (post == null)
                return RedirectToRoute("ViewFeed", new { userName = postViewModel.UserName, feedName = postViewModel.FeedName });
            Mapper.Map(postViewModel, post);
            posts.Update(post);
            return RedirectToRoute("ViewFeed", new { userName = postViewModel.UserName, feedName = postViewModel.FeedName });
        }
    }
}