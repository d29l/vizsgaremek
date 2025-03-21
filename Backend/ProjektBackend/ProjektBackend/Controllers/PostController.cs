using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjektBackend.Models;
using System.Configuration;
using System.Security.Claims;

namespace ProjektBackend.Controllers
{
    [Route("api/posts")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly ProjektContext _context;

        public PostController(ProjektContext context)
        {
            _context = context;
        }
        [Authorize]
        [HttpGet("fetchPosts")]
        public async Task<ActionResult<Post>> FetchPosts()
        {
            var posts = await _context.Posts.ToListAsync();
            if (posts != null && posts.Any())
            {
                return StatusCode(200, posts);
            }
            return StatusCode(404, "There are currently no Posts.");
        }
        [Authorize]
        [HttpGet("fetchPost/{PostId}")]
        public async Task<ActionResult<Post>> FetchPost(int PostId)
        {
            var post = await _context.Posts
                        .Include(x => x.Employer)
                        .FirstOrDefaultAsync(y => y.PostId == PostId);
            if (post != null)
            {
                return StatusCode(200, post);
            }
            return StatusCode(404, "No Post can be found with this Id.");
        }

        [Authorize(Policy = "EmployerSelfOrAdmin")]
        [HttpPost("newPost")]
        public async Task<ActionResult<Post>> NewPost(int EmployerId, int UserId, CreatePostDto createPostDto)
        {
            var Post = new Post()
            {
                EmployerId = EmployerId,
                UserId = UserId,
                Title = createPostDto.Title,
                Content = createPostDto.Content,
                CreatedAt = DateTime.Now,
                Likes = 0

            };

            if (Post != null)
            {
                _context.Add(Post);
                await _context.SaveChangesAsync();
                return StatusCode(201, "Post created successfully.");
            }
            return StatusCode(400, "Invalid data.");

        }

        [Authorize(Policy = "EmployerSelfOrAdmin")]
        [HttpPut("editPost/{PostId}")]

        public async Task<ActionResult<Post>> EditPost(int PostId, int EmployerId, int UserId, UpdatePostDto updatePostDto)
        {
            var existingPost = await _context.Posts.FirstOrDefaultAsync(x => x.PostId == PostId && x.EmployerId == EmployerId);
            if (existingPost != null)
            {
                existingPost.Title = updatePostDto.Title;
                existingPost.Content = updatePostDto.Content;
                existingPost.CreatedAt = DateTime.Now;
                _context.SaveChanges();
                return StatusCode(200, "Post updated.");
            }
            return StatusCode(404, "No Post can be found with this Id.");
        }

        [Authorize(Policy = "EmployerSelfOrAdmin")]
        [HttpDelete("deletePost/{PostId}")]
        public async Task<ActionResult> DeletePost(int PostId, int EmployerId, int UserId)
        {
            var deletePost = await _context.Posts
        .FirstOrDefaultAsync(x => x.PostId == PostId && x.EmployerId == EmployerId);

            if (deletePost != null)
            {
                _context.Remove(deletePost);
                await _context.SaveChangesAsync();
                return StatusCode(200,"Post successfully deleted.");
            }

            return StatusCode(404, "No Post can be found with this Id.");
        }
    }
}
