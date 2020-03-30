using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TweetBook.Data;
using TweetBook.Domain;
using TweetBook.Services.Abstract;

namespace TweetBook.Services.Implementation
{
    public class PostService : IPostService
    {
        private readonly ApplicationDbContext _dbContext;

        public PostService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// https://medium.com/bynder-tech/c-why-you-should-use-configureawait-false-in-your-library-code-d7837dce3d7f
        /// https://johnthiriet.com/configure-await/
        /// </summary>
        /// <param name="post"></param>
        /// <returns></returns>
        public async Task<bool> AddPost(Post post)
        {
            _dbContext.Posts.Add(post);
            try
            {
                int result = await _dbContext.SaveChangesAsync().ConfigureAwait(false);
                return result > 0;
            }
            catch (Exception ex)
            {

                throw;
            }
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<List<Post>> GetAllPost()
        {
            List<Post> posts= await _dbContext.Posts.ToListAsync().ConfigureAwait(false);
            return posts;

        }

        public async Task<Post> GetPostById(string postId)
        {
            return await _dbContext.Posts.SingleOrDefaultAsync(x => x.Id == postId).ConfigureAwait(false);
        }
    }
}
