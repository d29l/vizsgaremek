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
    }
}
