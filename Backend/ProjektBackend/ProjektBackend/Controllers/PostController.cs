using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
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
                var posts = await _context.Posts.Include(x => x.Employer).ToListAsync();
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
        [Authorize(Policy = "EmployerSelfOrAdmin")]
        [HttpPost("newPost")]
        public async Task<ActionResult<Post>> NewPost(CreatePostDto createPostDto, int EmployerId, int? userId = null)
        {
            try
            {
                int targetUserId;
                bool isAdmin = User.IsInRole("Admin");

                if (userId.HasValue && isAdmin)
                {
                    targetUserId = userId.Value;
                }
                else
                {
                    var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                    if (userIdClaim == null)
                        return StatusCode(401, "User ID not found in token.");

                    targetUserId = int.Parse(userIdClaim.Value);
                }

                var Post = new Post()
                {
                    EmployerId = EmployerId,
                    UserId = targetUserId,
                    Title = createPostDto.Title,
                    Content = createPostDto.Content,
                    Category = createPostDto.Category,
                    Location = createPostDto.Location,
                    CreatedAt = DateTime.Now
                };

                if (string.IsNullOrEmpty(createPostDto.Title) || string.IsNullOrEmpty(createPostDto.Content))
                {
                    return StatusCode(400, "Title and content are required.");
                }
                _context.Add(Post);
                await _context.SaveChangesAsync();
                return StatusCode(201, "Post created successfully.");
            }
            catch (DbUpdateException dbEx)
            {
                return StatusCode(409, $"Unable to create post: {dbEx.InnerException?.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while creating the post.");
            }
        }
        [Authorize(Policy = "EmployerSelfOrAdmin")]
        [HttpPut("editPost")]
        public async Task<ActionResult<Post>> EditPost(int PostId, int EmployerId, UpdatePostDto updatePostDto, int? userId = null)
        {
            try
            {
                int targetUserId;
                bool isAdmin = User.IsInRole("Admin");

                if (userId.HasValue && isAdmin)
                {
                    targetUserId = userId.Value;
                }
                else
                {
                    var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                    if (userIdClaim == null)
                        return StatusCode(401, "User ID not found in token.");

                    targetUserId = int.Parse(userIdClaim.Value);
                }

                var existingPost = await _context.Posts.FirstOrDefaultAsync(x => x.PostId == PostId && x.EmployerId == EmployerId);
                if (existingPost != null)
                {
                    existingPost.Title = updatePostDto.Title ?? existingPost.Title;
                    existingPost.Content = updatePostDto.Content ?? existingPost.Content;
                    existingPost.Category = updatePostDto.Category ?? existingPost.Category;
                    existingPost.Location = updatePostDto.Location ?? existingPost.Location;

                    existingPost.CreatedAt = DateTime.Now;

                    _context.Update(existingPost);
                    await _context.SaveChangesAsync();

                    return StatusCode(200, "Post updated.");
                }
                return StatusCode(404, "No Post can be found with this Id.");
            }
            catch (DbUpdateException dbEx)
            {
                return StatusCode(409, "Unable to update post.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while updating the post.");
            }
        }
        [Authorize(Policy = "EmployerSelfOrAdmin")]
        [HttpDelete("deletePost")]
        public async Task<ActionResult> DeletePost(int PostId, int EmployerId, int? userId = null)
        {
            try
            {
                int targetUserId;
                bool isAdmin = User.IsInRole("Admin");

                if (userId.HasValue && isAdmin)
                {
                    targetUserId = userId.Value;
                }
                else
                {
                    var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                    if (userIdClaim == null)
                        return StatusCode(401, "User ID not found in token.");

                    targetUserId = int.Parse(userIdClaim.Value);
                }

                var deletePost = await _context.Posts
                    .FirstOrDefaultAsync(x => x.PostId == PostId && x.EmployerId == EmployerId);

                if (deletePost != null)
                {
                    _context.Remove(deletePost);
                    await _context.SaveChangesAsync();
                    return StatusCode(200, "Post successfully deleted.");
                }
                return StatusCode(404, "No Post can be found with this Id.");
            }
            catch (DbUpdateException dbEx)
            {
                return StatusCode(409, "Unable to delete post due to related records.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while deleting the post.");
            }
        }
    }
}
