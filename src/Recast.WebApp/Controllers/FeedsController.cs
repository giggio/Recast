using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Recast.WebApp.Models;
using Recast.WebApp.Models.Entities;
using Recast.WebApp.Models.ViewModel;
using Recast.XmlFeed;

namespace Recast.WebApp.Controllers;

public class FeedsController : Controller
{
    private readonly Feeds feeds;
    private readonly Posts posts;
    private readonly IMapper mapper;

    public FeedsController(Feeds feeds, Posts posts, IMapper mapper)
    {
        this.feeds = feeds ?? throw new ArgumentNullException(nameof(feeds));
        this.posts = posts ?? throw new ArgumentNullException(nameof(posts));
        this.mapper = mapper;
    }

    [HttpGet]
    public ActionResult New() => View(new FeedViewModel());

    [HttpPost]
    public ActionResult New(FeedViewModel feedViewModel)
    {
        if (!ModelState.IsValid)
            return View(feedViewModel);
        var feed = new Feed(feedViewModel.UserName, feedViewModel.FeedName);

        var created = feeds.Insert(feed);
        if (created)
        {
            return RedirectToRoute("ViewFeed", new { userName = feedViewModel.UserName, feedName = feedViewModel.FeedName });
        }
        ViewBag.IsDuplicate = true;
        return View(feedViewModel);
    }

    public async Task<ActionResult> ViewFeed([FromServices] PostDeletionUsers usersRequiringKey, string userName, string feedName)
    {
        ViewBag.UserName = userName;
        ViewBag.FeedName = feedName;
        var feed = await feeds.Get(userName, feedName);
        if (feed == null)
            return View();
        var feedPosts = await posts.GetForFeed(feed);
        ViewBag.Posts = mapper.Map<IEnumerable<PostViewModel>>(feedPosts);
        ViewBag.CantDelete = usersRequiringKey.Users.Any(u => u == userName);
        return View(feed);
    }

    [HttpGet]
    public ActionResult AddPost(string userName, string feedName) => View(new PostViewModel { UserName = userName, FeedName = feedName, PublishDate = DateTime.Now, SongLink = "http://" });

    [HttpPost]
    public ActionResult AddPost(PostViewModel postView)
    {
        if (!ModelState.IsValid)
            return View(postView);

        var post = mapper.Map<Post>(postView);
        posts.Insert(post);

        return RedirectToRoute("ViewFeed", new { userName = post.GetUserName(), feedName = post.GetFeedName() });
    }
    [HttpGet]
    public ActionResult GoToFeed() => View();
    [HttpPost]
    public ActionResult GoToFeed(FeedViewModel feedViewModel) => !ModelState.IsValid
            ? View(feedViewModel)
            : RedirectToRoute("ViewFeed", new { userName = feedViewModel.UserName, feedName = feedViewModel.FeedName });

    public async Task<ActionResult> GetFeed(string userName, string feedName)
    {
        var feed = await feeds.Get(userName, feedName);
        var feedPosts = await posts.GetForFeed(feed);
        var xml = FeedConverter.Create(new Uri($"{Request.Scheme}://{Request.Host.Value}{Request.PathBase.Value}"), feed, feedPosts);
        return Content(xml, "text/xml");
    }

    public async Task<ActionResult> DeletePost([FromServices] PostDeletionUsers usersRequiringKey, [FromServices] PostDeletionKey deletionKey, string userName, string feedName, string title, string key = null)
    {
        if (usersRequiringKey.Users.Any(u => u == userName) && deletionKey.Key != key)
            return RedirectToRoute(nameof(ViewFeed), new { userName, feedName });
        await posts.Delete(userName, feedName, title);
        return RedirectToRoute(nameof(ViewFeed), new { userName, feedName });
    }

    [HttpGet]
    public async Task<ActionResult> EditPost(string userName, string feedName, string title)
    {
        var post = await posts.Get(userName, feedName, title);
        if (post == null)
            return RedirectToRoute("ViewFeed", new { userName, feedName });
        var postViewModel = mapper.Map<PostViewModel>(post);
        ViewBag.IsUpdate = true;
        return View(postViewModel);
    }

    [HttpPost]
    public async Task<ActionResult> EditPost(PostViewModel postViewModel)
    {
        if (!ModelState.IsValid)
            return View(postViewModel);
        var post = await posts.Get(postViewModel.UserName, postViewModel.FeedName, postViewModel.Title);
        if (post == null)
            return RedirectToRoute("ViewFeed", new { userName = postViewModel.UserName, feedName = postViewModel.FeedName });
        _ = mapper.Map(postViewModel, post);
        await posts.Update(post);
        return RedirectToRoute("ViewFeed", new { userName = postViewModel.UserName, feedName = postViewModel.FeedName });
    }
}
