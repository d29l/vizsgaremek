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
    [Route("api/employerrequests")]
    [ApiController]
    public class EmployerRequestController : ControllerBase
    {
        private readonly ProjektContext _context;

        public EmployerRequestController(ProjektContext context)
        {
            _context = context;
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpGet("fetchRequests")]
        public async Task<ActionResult<Employerrequest>> FetchRequests()
        {
            try
            {
                var requests = await _context.Employerrequests.ToListAsync();

                if (requests != null && requests.Any())
                {
                    return StatusCode(200, requests);
                }
                return StatusCode(404, "There are currently no Employer Requests.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while fetching employer requests.");
            }
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpGet("fetchRequest")]
        public async Task<ActionResult<Employerrequest>> FetchRequest(int? applicantId = null)
        {
            try
            {

                var request = await _context.Employerrequests.FirstOrDefaultAsync(x => x.ApplicantId == applicantId);

                if (request != null)
                {
                    return StatusCode(200, request);
                }
                return StatusCode(404, "No Request can be found with this Id.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while fetching the employer request.");
            }
        }

        [Authorize(Policy = "EmployeeSelfOrAdmin")]
        [HttpPost("postRequest")]
        public async Task<ActionResult> PostRequest(CreateRequestDto createRequestDto, int? userId = null)
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

                var newRequest = new Employerrequest
                {
                    UserId = targetUserId,
                    CompanyName = createRequestDto.CompanyName,
                    CompanyAddress = createRequestDto.CompanyAddress,
                    CompanyEmail = createRequestDto.CompanyEmail,
                    CompanyPhoneNumber = createRequestDto.CompanyPhoneNumber,
                    Industry = createRequestDto.Industry,
                    CompanyWebsite = createRequestDto.CompanyWebsite,
                    CompanyDescription = createRequestDto.CompanyDescription,
                    EstabilishedYear = createRequestDto.EstabilishedYear
                };

                if (newRequest != null)
                {
                    _context.Add(newRequest);
                    await _context.SaveChangesAsync();
                    return StatusCode(201, "Request created successfully.");
                }
                return StatusCode(400, "Invalid data.");
            }
            catch (DbUpdateException dbEx)
            {
                return StatusCode(409, "Unable to create request. It may already exist for this user.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while creating the employer request.");
            }
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpDelete("deleteRequest")]
        public async Task<ActionResult> DeleteRequest(int? applicantId = null)
        {
            try
            {
                int targetApplicantId;
                bool isAdmin = User.IsInRole("Admin");

                if (applicantId.HasValue && isAdmin)
                {
                    targetApplicantId = applicantId.Value;
                }
                else
                {
                    var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                    if (userIdClaim == null)
                        return StatusCode(401, "User ID not found in token.");

                    targetApplicantId = int.Parse(userIdClaim.Value);
                }

                var request = await _context.Employerrequests.FirstOrDefaultAsync(x => x.ApplicantId == targetApplicantId);

                if (request != null)
                {
                    _context.Remove(request);
                    await _context.SaveChangesAsync();
                    return StatusCode(200, "Request successfully deleted.");
                }
                return StatusCode(404, "No Request can be found with this Id.");
            }
            catch (DbUpdateException dbEx)
            {
                return StatusCode(409, "Unable to delete request due to database constraints.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while deleting the employer request.");
            }
        }
    }
}

