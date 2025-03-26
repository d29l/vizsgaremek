using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
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
        private readonly string _storagePath = Path.Combine(Directory.GetCurrentDirectory(), "Storage", "Images");

        public ProfileController(ProjektContext context)
        {
            _context = context;

            // Ensure Storage/Images directory exists
            if (!Directory.Exists(_storagePath))
            {
                Directory.CreateDirectory(_storagePath);
            }
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

        [Authorize(Policy = "SelfOrAdmin")]
        [HttpGet("fetchProfile")]
        public async Task<ActionResult<Profile>> FetchProfile(int? userId = null)
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

                var profile = await _context.Profiles.FirstOrDefaultAsync(x => x.UserId == targetUserId);

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

        [Authorize(Policy = "SelfOrAdmin")]
        [HttpPost("createProfile")]
        public async Task<ActionResult<Profile>> CreateProfile([FromForm] CreateProfileDto? createProfileDto, int? userId = null)
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

                var existingProfile = await _context.Profiles
                    .FirstOrDefaultAsync(p => p.UserId == targetUserId);

                if (existingProfile != null)
                {
                    return StatusCode(409, "A profile already exists for this user.");
                }

                string profilePictureUrl = createProfileDto.ProfilePicture != null
                    ? await SaveImageAsync(createProfileDto.ProfilePicture)
                    : "/Storage/Images/default.png";

                var profile = new Profile
                {
                    UserId = targetUserId,
                    Headline = createProfileDto.Headline ?? null,
                    Bio = createProfileDto.Bio ?? null,
                    Location = createProfileDto.Location ?? null,
                    ProfilePicture = profilePictureUrl
                };

                _context.Add(profile);
                await _context.SaveChangesAsync();

                return StatusCode(201, "Profile created successfully.");
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
        [HttpPut("updateProfile")]
        public async Task<ActionResult<Profile>> UpdateProfile([FromForm] UpdateProfileDto updateProfileDto, int? userId = null)
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

                var profile = await _context.Profiles
                    .FirstOrDefaultAsync(p => p.UserId == targetUserId);

                if (profile != null)
                {
                    profile.Headline = updateProfileDto.Headline ?? profile.Headline;
                    profile.Bio = updateProfileDto.Bio ?? profile.Bio;
                    profile.Location = updateProfileDto.Location ?? profile.Location;

                    if (updateProfileDto.ProfilePicture != null)
                    {
                        profile.ProfilePicture = await SaveImageAsync(updateProfileDto.ProfilePicture);
                    }

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

        private async Task<string> SaveImageAsync(IFormFile imageFile)
        {
            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
            string filePath = Path.Combine(_storagePath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }

            return $"/Storage/Images/{fileName}";
        }
    }
}
