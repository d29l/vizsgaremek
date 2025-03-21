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

        [HttpPost]

        public async Task<ActionResult<Profile>> PostProfile(int UserId, CreateProfileDto createProfileDto)
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
                ProfilePicture = "https://shorturl.at/Z5xgu"
            };
            if (profile != null)
            {
                _context.Profiles.Add(profile);
                await _context.SaveChangesAsync();

                return StatusCode(201, "Profile created successfully.");
            }
            return StatusCode(400, "Invalid data.");
        }


        [Authorize(Policy = "SelfOrAdmin")]
        [HttpPut("updateProfile/{ProfileId}")]
        public async Task<ActionResult<Profile>> UpdateProfile(int ProfileId, UpdateProfileDto updateProfileDto)
        {

            var profile = await _context.Profiles
                .FirstOrDefaultAsync(p => p.ProfileId == ProfileId);

            if (profile != null)
            {
                profile.Headline = updateProfileDto.Headline ?? profile.Headline;
                profile.Bio = updateProfileDto.Bio ?? profile.Bio;
                profile.Location = updateProfileDto.Location ?? profile.Location;
                profile.ProfilePicture = updateProfileDto.ProfilePicture ?? profile.ProfilePicture;

                _context.Profiles.Update(profile);
                await _context.SaveChangesAsync();

                return StatusCode(200, "Profile updated.");
            }

            return StatusCode(404, "No Profile can be found with this Id.");
        }
    }
}
