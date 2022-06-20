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
        var feed = await feeds.GetAsync(userName, feedName);
        if (feed == null)
            return View();
        var feedPosts = await posts.GetForFeed(feed);
        ViewBag.Posts = mapper.Map<IEnumerable<PostViewModel>>(feedPosts);
        ViewBag.CantDelete = usersRequiringKey.Users.Any(u => u == userName);
        return View(feed);
    }

    [HttpGet]
    public ActionResult AddPost([FromServices] PostDeletionUsers usersRequiringKey, string userName, string feedName)
    {
        ViewBag.NeedsKey = usersRequiringKey.Users.Any(u => u == userName);
        return View(new PostViewModel { UserName = userName, FeedName = feedName, PublishDate = DateTime.Now, SongLink = "http://" });
    }

    [HttpPost]
    public ActionResult AddPost([FromServices] PostDeletionUsers usersRequiringKey, [FromServices] PostDeletionKey creationKey, PostViewModel postView, string key)
    {
        var requiresKey = usersRequiringKey.Users.Any(u => u == postView.UserName);
        if (!ModelState.IsValid || requiresKey && creationKey.Key != key)
        {
            ViewBag.NeedsKey = usersRequiringKey.Users.Any(u => u == postView.UserName);
            ViewBag.Key = key;
            return View(postView);
        }

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
        var feed = await feeds.GetAsync(userName, feedName);
        var feedPosts = await posts.GetForFeed(feed);
        var xml = FeedConverter.Create(new Uri($"{Request.Scheme}://{Request.Host.Value}{Request.PathBase.Value}"), feed, feedPosts);
        return Content(xml, "text/xml");
    }

    [HttpPost]
    public async Task<ActionResult> DeletePost([FromServices] PostDeletionUsers usersRequiringKey, [FromServices] PostDeletionKey deletionKey, string userName, string feedName, string title, string key = null)
    {
        if (usersRequiringKey.Users.Any(u => u == userName) && deletionKey.Key != key)
            return RedirectToRoute(nameof(ViewFeed), new { userName, feedName });
        await posts.Delete(userName, feedName, title);
        return RedirectToRoute(nameof(ViewFeed), new { userName, feedName });
    }

    [HttpGet]
    public async Task<ActionResult> EditPost([FromServices] PostDeletionUsers usersRequiringKey, string userName, string feedName, string title)
    {
        ViewBag.NeedsKey = usersRequiringKey.Users.Any(u => u == userName);
        var post = await posts.Get(userName, feedName, title);
        if (post == null)
            return RedirectToRoute("ViewFeed", new { userName, feedName });
        var postViewModel = mapper.Map<PostViewModel>(post);
        ViewBag.IsUpdate = true;
        return View(postViewModel);
    }

    [HttpPost]
    public async Task<ActionResult> EditPost([FromServices] PostDeletionUsers usersRequiringKey, [FromServices] PostDeletionKey editKey, PostViewModel postViewModel, string key = null)
    {
        if (!ModelState.IsValid || usersRequiringKey.Users.Any(u => u == postViewModel.UserName) && editKey.Key != key)
        {
            ViewBag.NeedsKey = usersRequiringKey.Users.Any(u => u == postViewModel.UserName);
            ViewBag.Key = key;
            ViewBag.IsUpdate = true;
            return View(postViewModel);
        }
        var post = await posts.Get(postViewModel.UserName, postViewModel.FeedName, postViewModel.Title);
        if (post == null)
            return RedirectToRoute("ViewFeed", new { userName = postViewModel.UserName, feedName = postViewModel.FeedName });
        _ = mapper.Map(postViewModel, post);
        await posts.Update(post);
        return RedirectToRoute("ViewFeed", new { userName = postViewModel.UserName, feedName = postViewModel.FeedName });
    }
}
