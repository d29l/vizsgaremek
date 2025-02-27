using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjektBackend.Models;
using System.Security.Claims;

namespace ProjektBackend.Controllers
{
    [Route("api/profiles")]
    [ApiController]
    public class ProfileController : ControllerBase
    {

        private readonly ProjektContext _context;

        public ProfileController(ProjektContext context)
        {
            _context = context;
        }

        //Post
        [HttpPost]

        public async Task<ActionResult<Profile>> postProfile(int UserId, CreateProfileDto createProfileDto)
        {
            var existingProfile = await _context.Profiles
                .FirstOrDefaultAsync(p => p.UserId == UserId);

            if (existingProfile != null)
            {
                return Conflict(new { Message = "A profile already exists for this user." });
            }

            var profile = new Profile
            {
                UserId = UserId,
                Headline = createProfileDto.Headline,
                Bio = createProfileDto.Bio,
                Location = createProfileDto.Location,
                ProfilePicture = createProfileDto.ProfilePicture
            };
            if (profile != null)
            {
                _context.Profiles.Add(profile);
                await _context.SaveChangesAsync();

                return Ok(profile);
            }
            return BadRequest();
        }


        //Put
        [Authorize(Policy = "SelfOrAdmin")]
        [HttpPut("updateProfile/{ProfileId}")]
        public async Task<ActionResult<Profile>> updateProfile(int ProfileId, UpdateProfileDto updateProfileDto)
        {

            var profile = await _context.Profiles
                .FirstOrDefaultAsync(p => p.ProfileId == ProfileId);

            profile.Headline = updateProfileDto.Headline ?? profile.Headline;
            profile.Bio = updateProfileDto.Bio ?? profile.Bio;
            profile.Location = updateProfileDto.Location ?? profile.Location;
            profile.ProfilePicture = updateProfileDto.ProfilePicture ?? profile.ProfilePicture;

            if (profile != null)
            {
                _context.Profiles.Update(profile);
                await _context.SaveChangesAsync();

                return Ok(profile);
            }

            return BadRequest();
        }
    }
}
