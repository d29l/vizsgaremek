using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjektBackend.Models;
using System.Configuration;

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
                        .Include(x => x.User)
                        .ThenInclude(e => e.Employers)
                        .FirstOrDefaultAsync(y => y.PostId == PostId);
            if (post != null)
            {
                return Ok(post);
            }
            return NotFound();
        }

        // Post
        [Authorize(Policy = "EmployerOnly,AdminOnly")]
        [HttpPost("newPost")]
        public async Task<ActionResult<Post>> newPost(int UserId, CreatePostDto createPostDto)
        {
            var Post = new Post()
            {
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
        [Authorize(Policy = "EmployerSelfOnly,AdminOnly")]
        [HttpPut("editPost")]

        public async Task<ActionResult<Post>> editPost(int PostId, int UserId, UpdatePostDto updatePostDto)
        {
            var existingPost = await _context.Posts.FirstOrDefaultAsync(x => x.PostId == PostId && x.UserId == UserId);
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
        [Authorize(Policy = "EmployerSelfOnly,AdminOnly")]
        [HttpDelete("deletePost")]

        public async Task<ActionResult> deletePost(int PostId, int UserId) 
        {
            var deletePost = await _context.Posts.FirstOrDefaultAsync(x => x.PostId == PostId && x.UserId == UserId);
            if (deletePost != null)
            {
                _context.Remove(deletePost);
                await _context.SaveChangesAsync();
                return StatusCode(200, "User successfully deleted.");
            }
            return NotFound();
        }
    }
}
