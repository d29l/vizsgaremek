using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjektBackend.Models;

namespace ProjektBackend.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ProjektContext _context;

        public UserController(ProjektContext context)
        {
            _context = context;
        }

        // Get All

        [HttpGet("fetchUsers")]
        public async Task<ActionResult<User>> fetchUsers()
        {
            var users = await _context.Users.ToListAsync();
            if (users != null)
            {
                return Ok(users);
            }
            return BadRequest();
        }

        // Get Id

        [HttpGet("fetchUser/{UserId}")]
        public async Task<ActionResult<User>> fetchUser(int UserId)
        {
            var user = await _context.Users.Include(p => p.Employers).FirstOrDefaultAsync(p => p.UserId == UserId);
            if (user != null)
            {
                return Ok(user);
            }
            return NotFound();
        }


    }
}
