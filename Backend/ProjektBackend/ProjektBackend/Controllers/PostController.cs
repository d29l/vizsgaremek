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

        // Get All

        [HttpGet("fetchPosts")]
        public async Task<ActionResult<Post>> fetchPosts()
        {
            var posts = await _context.Posts.ToListAsync();
            if (posts != null)
            {
                return Ok(posts);
            }
            return BadRequest();
        }

        // Get Id

        [HttpGet("fetchPost/{PostId}")]
        public async Task<ActionResult<Post>> fetchPost(int PostId)
        {
            var post = await _context.Posts
                        .Include(x => x.Employer)
                        .FirstOrDefaultAsync(y => y.PostId == PostId);
            if (post != null)
            {
                return Ok(post);
            }
            return NotFound();
        }

        // Post
        [Authorize(Policy = "EmployerSelfOrAdmin")]
        [HttpPost("newPost")]
        public async Task<ActionResult<Post>> newPost(int EmployerId, int UserId, CreatePostDto createPostDto)
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
                return StatusCode(201, Post);
            }
            return BadRequest();

        }

        // Put
        [Authorize(Policy = "EmployerSelfOrAdmin")]
        [HttpPut("editPost/{PostId}")]

        public async Task<ActionResult<Post>> editPost(int PostId, int EmployerId, UpdatePostDto updatePostDto)
        {
            var existingPost = await _context.Posts.FirstOrDefaultAsync(x => x.PostId == PostId && x.EmployerId == EmployerId);
            if (existingPost != null)
            {
                existingPost.Title = updatePostDto.Title;
                existingPost.Content = updatePostDto.Content;
                existingPost.CreatedAt = DateTime.Now;
                _context.SaveChanges();
                return StatusCode(200, existingPost);
            }
            return NotFound();
        }

        // Delete
        [Authorize(Policy = "EmployerSelfOrAdmin")]
        [HttpDelete("deletePost/{PostId}")]
        public async Task<ActionResult> DeletePost(int PostId, int EmployerId)
        {
            var deletePost = await _context.Posts
        .FirstOrDefaultAsync(x => x.PostId == PostId && x.EmployerId == EmployerId);

            if (deletePost != null)
            {
                _context.Remove(deletePost);
                await _context.SaveChangesAsync();
                return Ok("Post successfully deleted");
            }

            return NotFound();
        }
    }
}
