using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjektBackend.Models;

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
        public async Task<ActionResult<Employerrequest>> fetchRequests()
        {
            var requests = await _context.Employerrequests.ToListAsync();

            if (requests != null)
            {
                return Ok(requests);
            }
            return NotFound();
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpGet("fetchRequest/{ApplicantId}")]
        public async Task<ActionResult<Employerrequest>> fetchRequest(int ApplicantId)
        {
            var request = await _context.Employerrequests.FirstOrDefaultAsync(x => x.ApplicantId == ApplicantId);

            if (request != null)
            {
                return Ok(request);
            }
            return NotFound();
        }

        [Authorize(Policy = "EmployeeOnly")]
        [HttpPost("postRequest")]

        public async Task<ActionResult> postRequest(CreateRequestDto createRequestDto)
        {

            var newRequest = new Employerrequest
            {
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
                return StatusCode(201, "Request successfully created.");
            }
            return BadRequest();
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpDelete("deleteRequest/{ApplicantId}")]

        public async Task<ActionResult> deleteRequest(int ApplicantId)
        {
            var request = await _context.Employerrequests.FirstOrDefaultAsync(x => x.ApplicantId == ApplicantId);

            if (request != null)
            {
                _context.Remove(request);
                await _context.SaveChangesAsync();
                return Ok("User successfully deleted.");
            }
            return BadRequest();
        }

    }
}
