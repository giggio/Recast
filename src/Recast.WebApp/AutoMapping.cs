using AutoMapper;
using Recast.WebApp.Models.Entities;
using Recast.WebApp.Models.ViewModel;
using System;
using System.Web;

namespace Recast.WebApp
{
    public class AutoMapping
    {
        public static void MapAll()
        {
            Mapper.Initialize(config =>
            {
                config.CreateMap<PostViewModel, Post>()
                    .ForMember(p => p.Duration, opt => opt.MapFrom(pvm => pvm.Duration.Ticks))
                    .ForMember(p => p.PartitionKey, opt => opt.MapFrom(pvm => Post.CreateKey(pvm.UserName, pvm.FeedName)))
                    .ForMember(p => p.RowKey, opt => opt.MapFrom(pvm => HttpUtility.UrlEncode(pvm.Title)));
                config.CreateMap<Post, PostViewModel>()
                    .ForMember(pvm => pvm.Duration, opt => opt.MapFrom(p => new TimeSpan(p.Duration)));
            });
        }
    }
}