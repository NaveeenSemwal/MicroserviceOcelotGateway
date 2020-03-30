using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TweetBook.Domain;

namespace TweetBook.Services.Abstract
{
    public interface IPostService
    {
        Task<List<Post>> GetAllPost();

        Task<Post> GetPostById(string postId);

        Task<bool> AddPost(Post post);
    }
}
