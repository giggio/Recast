using Recast.WebApp.Models.Entities;
using Recast.WebApp.Models.ViewModel;
using System.Web;

namespace Recast.WebApp;

public static class AutoMapping
{
    public static void MapAll(this IServiceCollection services)
        => services.AddAutoMapper(config =>
        {
            config.CreateMap<PostViewModel, Post>()
                .ForMember(p => p.Duration, opt => opt.MapFrom(pvm => pvm.Duration.TotalSeconds))
                .ForMember(p => p.PartitionKey, opt => opt.MapFrom(pvm => Post.CreateKey(pvm.UserName, pvm.FeedName)))
                .ForMember(p => p.RowKey, opt => opt.MapFrom(pvm => HttpUtility.UrlEncode(pvm.Title)));
            config.CreateMap<Post, PostViewModel>()
                .ForMember(pvm => pvm.Duration, opt => opt.MapFrom(p => TimeSpan.FromSeconds(p.Duration)));
        }, typeof(AutoMapping).Assembly);
}
