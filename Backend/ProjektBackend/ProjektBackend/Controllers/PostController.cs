using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjektBackend.Models;
using System.Configuration;
using System.Security.Claims;

#pragma warning disable CS0168

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
            try
            {
                var posts = await _context.Posts.ToListAsync();
                if (posts != null && posts.Any())
                {
                    return StatusCode(200, posts);
                }
                return StatusCode(404, "There are currently no Posts.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while fetching posts.");
            }
        }

        [Authorize]
        [HttpGet("fetchPost/{PostId}")]
        public async Task<ActionResult<Post>> FetchPost(int PostId)
        {
            try
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
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while fetching the post.");
            }
        }
        // Faulty
        [Authorize(Policy = "EmployerSelfOrAdmin")]
        [HttpPost("newPost")]
        public async Task<ActionResult<Post>> NewPost(int EmployerId, int UserId, CreatePostDto createPostDto)
        {
            try
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
            catch (DbUpdateException dbEx)
            {
                return StatusCode(409, "Unable to create post.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while creating the post.");
            }
        }

    }
}
