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
                    if (string.IsNullOrEmpty(createPostDto.Title) || string.IsNullOrEmpty(createPostDto.Content))
                    {
                        return StatusCode(400, "Title and content are required.");
                    }
                    _context.Add(Post);
                    await _context.SaveChangesAsync();
                    return StatusCode(201, "Post created successfully.");
                }
                return StatusCode(400, "Invalid data.");
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
        [HttpPut("editPost/{PostId}")]
        public async Task<ActionResult<Post>> EditPost(int PostId, int EmployerId, int UserId, UpdatePostDto updatePostDto)
        {
            try
            {
                var existingPost = await _context.Posts.FirstOrDefaultAsync(x => x.PostId == PostId && x.EmployerId == EmployerId);
                if (existingPost != null)
                {
                    existingPost.Title = updatePostDto.Title ?? existingPost.Title;
                    existingPost.Content = updatePostDto.Content ?? existingPost.Content;

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
        [HttpDelete("deletePost/{PostId}")]
        public async Task<ActionResult> DeletePost(int PostId, int EmployerId, int UserId)
        {
            try
            {
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
