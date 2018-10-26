using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Recast.WebApp.Models;
using Recast.WebApp.Models.Entities;
using Recast.WebApp.Models.ViewModel;
using Recast.XmlFeed;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Recast.WebApp.Controllers
{
    public class FeedsController : Controller
    {
        private readonly Feeds feeds;
        private readonly Posts posts;

        public FeedsController(Feeds feeds, Posts posts)
        {
            this.feeds = feeds ?? throw new ArgumentNullException(nameof(feeds));
            this.posts = posts ?? throw new ArgumentNullException(nameof(posts));
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

        public async Task<ActionResult> ViewFeed(string userName, string feedName)
        {
            ViewBag.UserName = userName;
            ViewBag.FeedName = feedName;
            var feed = await feeds.Get(userName, feedName);
            if (feed == null)
                return View();
            var feedPosts = await posts.GetForFeed(feed);
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

        public async Task<ActionResult> GetFeed(string userName, string feedName)
        {
            var feed = await feeds.Get(userName, feedName);
            var feedPosts = await posts.GetForFeed(feed);
            var xml = FeedConverter.Create(new Uri($"{Request.Scheme}://{Request.Host.Value}{Request.PathBase.Value}"), feed, feedPosts);
            return Content(xml, "text/xml");
        }

        public async Task<ActionResult> DeletePost(string userName, string feedName, string title)
        {
            await posts.Delete(userName, feedName, title);
            return RedirectToRoute("ViewFeed", new { userName, feedName });
        }

        [HttpGet]
        public async Task<ActionResult> EditPost(string userName, string feedName, string title)
        {
            var post = await posts.Get(userName, feedName, title);
            if (post == null)
                return RedirectToRoute("ViewFeed", new { userName, feedName });
            var postViewModel = Mapper.Map<PostViewModel>(post);
            ViewBag.IsUpdate = true;
            return View(postViewModel);
        }

        [HttpPost]
        public async Task<ActionResult> EditPost(PostViewModel postViewModel)
        {
            if (!ModelState.IsValid) return View(postViewModel);
            var post = await posts.Get(postViewModel.UserName, postViewModel.FeedName, postViewModel.Title);
            if (post == null)
                return RedirectToRoute("ViewFeed", new { userName = postViewModel.UserName, feedName = postViewModel.FeedName });
            Mapper.Map(postViewModel, post);
            await posts.Update(post);
            return RedirectToRoute("ViewFeed", new { userName = postViewModel.UserName, feedName = postViewModel.FeedName });
        }
    }
}