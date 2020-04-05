using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TweetBook.Contracts.V1;
using TweetBook.Contracts.V1.Request;
using TweetBook.Contracts.V1.Response;
using TweetBook.Domain;
using TweetBook.Extension;
using TweetBook.Services.Abstract;

namespace TweetBook.Controllers.V1
{
    [Route(ApiRoutes.ControllerRoute)]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly IPostService _postService;
        private readonly IMapper _mapper;

        public PostController(IPostService postService, IMapper mapper)
        {
            _postService = postService;
            _mapper = mapper;
        }


        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet(ApiRoutes.Posts.GetAll)]
        public async Task<IActionResult> GetAll()
        {
            var posts = await _postService.GetAllPost().ConfigureAwait(false);
            var postResponse = _mapper.Map<List<PostResponse>>(posts);

            return Ok(postResponse);
        }


        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet(ApiRoutes.Posts.Get)]
        public async Task<IActionResult> Get([FromRoute]string postId)
        {
            var post = await _postService.GetPostById(postId).ConfigureAwait(false);

            if (post == null)
            {
                return NotFound();
            }

            var postResponse = _mapper.Map<PostResponse>(post);

            return Ok(postResponse);
        }


        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost(ApiRoutes.Posts.Create)]
        public IActionResult Create([FromBody] PostRequest request)
        {
            if (request == null)
            {
                throw new Exception("post request cannot be null");
            }


            request.PostId = Guid.NewGuid().ToString();


            // Mapping request data to domain models
            var post = _mapper.Map<Post>(request);
            post.UserId = HttpContext.GetUserId();

            _postService.AddPost(post);

            return CreatedAtAction("Get", new { id = post.Id }, _mapper.Map<PostResponse>(post));
        }
    }
}