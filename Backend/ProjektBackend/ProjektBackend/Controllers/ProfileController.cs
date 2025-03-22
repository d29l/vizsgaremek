using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjektBackend.Models;
using System.Security.Claims;

#pragma warning disable CS8604
#pragma warning disable CS0168

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

        [Authorize(Policy = "AdminOnly")]
        [HttpGet("fetchProfiles")]

        public async Task<ActionResult<Profile>> FetchProfiles()
        {
            try
            {
                var profiles = await _context.Profiles.ToListAsync();

                if (profiles != null && profiles.Any())
                {
                    return StatusCode(200, profiles);
                }
                return StatusCode(404, "There are currently no Profiles.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while fetching profiles.");
            }
        }

        [Authorize]
        [HttpGet("fetchProfile/{ProfileId}")]

        public async Task<ActionResult<Profile>> FetchProfile(int ProfileId)
        {
            try
            {
                var profile = await _context.Profiles.FirstOrDefaultAsync(x => x.ProfileId == ProfileId);

                if (profile != null)
                {
                    return StatusCode(200, profile);
                }
                return StatusCode(404, "No Profile can be found with this Id.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while fetching the profile.");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Profile>> PostProfile(int UserId, CreateProfileDto createProfileDto)
        {
            try
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
            catch (DbUpdateException dbEx)
            {
                return StatusCode(409, "Unable to create profile. It may already exist.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while creating the profile.");
            }
        }

        [Authorize(Policy = "SelfOrAdmin")]
        [HttpPut("updateProfile/{ProfileId}")]
        public async Task<ActionResult<Profile>> UpdateProfile(int ProfileId, int UserId, UpdateProfileDto updateProfileDto)
        {
            try
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
            catch (DbUpdateException dbEx)
            {
                return StatusCode(409, "Unable to update profile.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while updating the profile.");
            }
        }

    }
}
