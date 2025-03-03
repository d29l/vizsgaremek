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

        public async Task<ActionResult<Employer>> fetchEmployers()
        {
            var employers = await _context.Employers.ToListAsync();

            if (employers != null)
            {
                return Ok(employers);
            }
            return BadRequest();
        }

        [Authorize(Policy = "EmployerSelfOrAdmin")]
        [HttpGet("fetchEmployer{EmployerId}")]

        public async Task<ActionResult<Employer>> fetchEmployers(int EmployerId)
        {
            var employer = await _context.Employers.FirstOrDefaultAsync(x => x.EmployerId == EmployerId);

            if (employer != null)
            {
                return Ok(employer);
            }
            return BadRequest();
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpPost("postEmployer")]

        public async Task<ActionResult> postEmployer(CreateEmployerDto createEmployerDto)
        {
            var newEmployer = new Employer
            {
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
                return StatusCode(201, "Employer successfully created.");
            }
            return BadRequest();
        }

        [Authorize(Policy = "EmployerSelfOrAdmin")]
        [HttpPut("editEmployer{EmployerId}")]

        public async Task<ActionResult> editEmployer(int EmployerId, UpdateEmployerDto updateEmployerDto)
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
                return Ok(existingEmployer);
            }
            return BadRequest();

            
        }

    }
}
