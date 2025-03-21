using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjektBackend.Models;

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

        [Authorize(Policy = "AdminOnly")]
        [HttpGet("fetchEmployers")]

        public async Task<ActionResult<Employer>> FetchEmployers()
        {
            var employers = await _context.Employers.ToListAsync();

            if (employers != null && employers.Any())
            {
                return StatusCode(200, employers);
            }
            return StatusCode(404, "There are currently no Employers.");
        }

        [Authorize(Policy = "EmployerSelfOrAdmin")]
        [HttpGet("fetchEmployer/{EmployerId}")]

        public async Task<ActionResult<Employer>> FetchEmployer(int EmployerId)
        {
            var employer = await _context.Employers.FirstOrDefaultAsync(x => x.EmployerId == EmployerId);

            if (employer != null)
            {
                return StatusCode(200, employer);
            }
            return StatusCode(404, "No Employer can be found with this Id.");
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpPost("postEmployer")]

        public async Task<ActionResult> PostEmployer(int UserId, CreateEmployerDto createEmployerDto)
        {
            var newEmployer = new Employer
            {
                UserId = UserId,
                CompanyName = createEmployerDto.CompanyName,
                CompanyAddress = createEmployerDto.CompanyAddress,
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

        [Authorize(Policy = "EmployerSelfOrAdmin")]
        [HttpPut("editEmployer/{EmployerId}")]

        public async Task<ActionResult> EditEmployer(int EmployerId, UpdateEmployerDto updateEmployerDto)
        {
            var existingEmployer = await _context.Employers.FirstOrDefaultAsync(x => x.EmployerId == EmployerId);

            if (existingEmployer != null) 
            {
                existingEmployer.CompanyName = updateEmployerDto.CompanyName;
                existingEmployer.CompanyAddress = updateEmployerDto.CompanyAddress;
                existingEmployer.Industry = updateEmployerDto.Industry;
                existingEmployer.CompanyWebsite = updateEmployerDto.CompanyWebsite;
                existingEmployer.CompanyDescription = updateEmployerDto.CompanyDescription;

                _context.Update(existingEmployer);
                await _context.SaveChangesAsync();
                return StatusCode(200, "Employer updated.");
            }
            return StatusCode(404, "No Employer can be found with this Id.");

            
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpDelete("deleteEmployer/{EmployerId}")]

        public async Task<ActionResult> DeleteEmployer(int EmployerId)
        {
            var employer = await _context.Employers.FirstOrDefaultAsync(x => x.EmployerId == EmployerId);

            if (employer != null)
            {
                _context.Remove(employer);
                await _context.SaveChangesAsync();
                return StatusCode(200, "Employer successfully deleted.");
            }
            return StatusCode(404, "No Employer can be found with this Id.");
        }


    }
}
