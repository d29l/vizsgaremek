using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using ProjektBackend.Models;
using System.Security.Claims;

#pragma warning disable CS0168

namespace ProjektBackend.Controllers
{
    [Route("api/employers")]
    [ApiController]
    public class EmployerController : ControllerBase
    {
        private readonly ProjektContext _context;

        public EmployerController(ProjektContext context)
        {
            _context = context;
        }

        [Authorize]
        [HttpGet("fetchEmployers")]
        public async Task<ActionResult<Employer>> FetchEmployers()
        {
            try
            {
                var employers = await _context.Employers.ToListAsync();

                if (employers != null && employers.Any())
                {
                    return StatusCode(200, employers);
                }
                return StatusCode(404, "There are currently no Employers.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while fetching employers.");
            }
        }
        [Authorize]
        [HttpGet("fetchEmployer")]
        public async Task<ActionResult<Employer>> FetchEmployer(int? userId = null)
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

                var employer = await _context.Employers.FirstOrDefaultAsync(x => x.UserId == targetUserId);

                if (employer != null)
                {
                    return StatusCode(200, employer);
                }
                return StatusCode(404, "No Employer can be found with this Id.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while fetching the employer.");
            }
        }
        [Authorize(Policy = "AdminOnly")]
        [HttpPost("postEmployer")]
        public async Task<ActionResult> PostEmployer(CreateEmployerDto createEmployerDto, int? userId = null)
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

                var newEmployer = new Employer
                {
                    UserId = targetUserId,
                    CompanyName = createEmployerDto.CompanyName,
                    CompanyAddress = createEmployerDto.CompanyAddress,
                    CompanyEmail = createEmployerDto.CompanyEmail,
                    CompanyPhoneNumber = createEmployerDto.CompanyPhoneNumber,
                    Industry = createEmployerDto.Industry,
                    CompanyWebsite = createEmployerDto.CompanyWebsite,
                    CompanyDescription = createEmployerDto.CompanyDescription,
                    EstablishedYear = createEmployerDto.EstabilishedYear
                };

                if (newEmployer != null)
                {
                    _context.Add(newEmployer);
                    await _context.SaveChangesAsync();
                    return StatusCode(201, "Employer created successfully.");
                }
                return StatusCode(400, "Invalid data.");
            }
            catch (DbUpdateException dbEx)
            {
                return StatusCode(409, "Unable to create employer. It may already exist.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while creating the employer.");
            }
        }
        [Authorize(Policy = "EmployerSelfOrAdmin")]
        [HttpPut("editEmployer")]
        public async Task<ActionResult> EditEmployer(UpdateEmployerDto updateEmployerDto, int? userId = null)
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

                var existingEmployer = await _context.Employers.FirstOrDefaultAsync(x => x.UserId == targetUserId);

                if (existingEmployer != null)
                {
                    existingEmployer.CompanyName = updateEmployerDto.CompanyName ?? existingEmployer.CompanyName;
                    existingEmployer.CompanyAddress = updateEmployerDto.CompanyAddress ?? existingEmployer.CompanyAddress;
                    existingEmployer.CompanyEmail = updateEmployerDto.CompanyEmail ?? existingEmployer.CompanyEmail;
                    existingEmployer.CompanyPhoneNumber = updateEmployerDto.CompanyPhoneNumber ?? existingEmployer.CompanyPhoneNumber;
                    existingEmployer.Industry = updateEmployerDto.Industry ?? existingEmployer.Industry;
                    existingEmployer.CompanyWebsite = updateEmployerDto.CompanyWebsite ?? existingEmployer.CompanyWebsite;
                    existingEmployer.CompanyDescription = updateEmployerDto.CompanyDescription ?? existingEmployer.CompanyDescription;

                    _context.Update(existingEmployer);
                    await _context.SaveChangesAsync();
                    return StatusCode(200, "Employer updated.");
                }
                return StatusCode(404, "No Employer can be found with this Id.");
            }
            catch (DbUpdateException dbEx)
            {
                return StatusCode(409, "Unable to update employer information.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while updating the employer.");
            }
        }
        [Authorize(Policy = "AdminOnly")]
        [HttpDelete("deleteEmployer")]
        public async Task<ActionResult> DeleteEmployer(int? userId = null)
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

                var employer = await _context.Employers.FirstOrDefaultAsync(x => x.UserId == targetUserId);

                if (employer != null)
                {
                    _context.Remove(employer);
                    await _context.SaveChangesAsync();
                    return StatusCode(200, "Employer successfully deleted.");
                }
                return StatusCode(404, "No Employer can be found with this Id.");
            }
            catch (DbUpdateException dbEx)
            {
                return StatusCode(409, "Unable to delete employer due to related records.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while deleting the employer.");
            }
        }
    }
}
