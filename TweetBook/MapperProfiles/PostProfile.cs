using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TweetBook.Contracts.V1.Request;
using TweetBook.Contracts.V1.Response;
using TweetBook.Domain;

namespace TweetBook.MapperProfiles
{
    public class PostProfile : Profile
    {
        public PostProfile()
        {
            CreateMap<PostRequest, Post>()
                .ForMember(dest => dest.Id,
                 opt => opt.MapFrom(src => src.PostId))
                .ReverseMap();


            CreateMap<Post, PostResponse>();
        }
    }
}
