using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjektBackend.Models;

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
        [HttpGet("fetchRequest/{ApplicantId}")]
        public async Task<ActionResult<Employerrequest>> FetchRequest(int ApplicantId)
        {
            try
            {
                var request = await _context.Employerrequests.FirstOrDefaultAsync(x => x.ApplicantId == ApplicantId);

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
        public async Task<ActionResult> PostRequest(int UserId, CreateRequestDto createRequestDto)
        {
            try
            {
                var newRequest = new Employerrequest
                {
                    UserId = UserId,
                    CompanyName = createRequestDto.CompanyName,
                    CompanyAddress = createRequestDto.CompanyAddress,
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
        [HttpDelete("deleteRequest/{ApplicantId}")]
        public async Task<ActionResult> DeleteRequest(int ApplicantId)
        {
            try
            {
                var request = await _context.Employerrequests.FirstOrDefaultAsync(x => x.ApplicantId == ApplicantId);

                if (request != null)
                {
                    _context.Remove(request);
                    await _context.SaveChangesAsync();
                    return StatusCode(200, "User successfully deleted.");
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
