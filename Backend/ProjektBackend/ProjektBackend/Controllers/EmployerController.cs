﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjektBackend.Models;

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

        [Authorize(Policy = "AdminOnly")]
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

        [Authorize(Policy = "EmployerSelfOrAdmin")]
        [HttpGet("fetchEmployer/{EmployerId}")]
        public async Task<ActionResult<Employer>> FetchEmployer(int EmployerId)
        {
            try
            {
                var employer = await _context.Employers.FirstOrDefaultAsync(x => x.EmployerId == EmployerId);

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
        public async Task<ActionResult> PostEmployer(int UserId, CreateEmployerDto createEmployerDto)
        {
            try
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
            catch (DbUpdateException dbEx)
            {
                return StatusCode(409, "Unable to create employer. It may already exist.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while creating the employer.");
            }
        }



    }
}
