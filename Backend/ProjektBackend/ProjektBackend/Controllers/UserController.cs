﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ProjektBackend.Models;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;


namespace ProjektBackend.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ProjektContext _context;
        private readonly IConfiguration _configuration;

        public UserController(ProjektContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // Get All
        [Authorize(Policy = "AdminOnly")]
        [HttpGet("fetchUsers")]
        public async Task<ActionResult<User>> fetchUsers()
        {
            var claims = User.Claims;
            foreach (var claim in claims)
            {
                Console.WriteLine($"{claim.Type}: {claim.Value}");
            }


            var users = await _context.Users.ToListAsync();
            if (users != null)
            {
                return Ok(users);
            }
            return BadRequest();
        }

        // Get Id

        [Authorize(Policy = "SelfOrAdmin")]
        [HttpGet("fetchUser/{UserId}")]
        public async Task<ActionResult<User>> fetchUser(int UserId)
        {
            var user = await _context.Users.Include(p => p.Employers).FirstOrDefaultAsync(p => p.UserId == UserId);
            if (user != null)
            {
                return Ok(user);
            }
            return NotFound();
        }

        // Register

        [HttpPost("registerUser")]
        public async Task<ActionResult<User>> registerUser(RegisterUserDto registerUserDto)
        {
            byte[] salt = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            var pbkdf2 = new Rfc2898DeriveBytes(registerUserDto.Password, salt, 100000, HashAlgorithmName.SHA256);
            byte[] hash = pbkdf2.GetBytes(32);

            byte[] hashBytes = new byte[48];
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 32);

            var newUser = new User
            {
                FirstName = registerUserDto.FirstName,
                LastName = registerUserDto.LastName,
                Email = registerUserDto.Email,
                Password = Convert.ToBase64String(hashBytes)
            };

            if (newUser != null)
            {
                _context.Add(newUser);
                await _context.SaveChangesAsync();
                return StatusCode(201, newUser);
            }

            return BadRequest();
        }



        //Login
        [HttpPost("loginUser")]

        public async Task<ActionResult<string>> loginUser(LoginUserDto loginUserDto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == loginUserDto.Email);

            if (user == null)
            {
                return Unauthorized(new { Message = "Invalid email or password." });
            }

            byte[] hashBytes = Convert.FromBase64String(user.Password);

            byte[] salt = new byte[16];
            Array.Copy(hashBytes, 0, salt, 0, 16);

            var pbkdf2 = new Rfc2898DeriveBytes(loginUserDto.Password, salt, 100000, HashAlgorithmName.SHA256);
            byte[] newHash = pbkdf2.GetBytes(32);

            bool isPasswordValid = true;
            for (int i = 0; i < 32; i++)
            {
                if (hashBytes[i + 16] != newHash[i])
                {
                    isPasswordValid = false;
                    break;
                }
            }

            if (!isPasswordValid)
            {
                return Unauthorized(new { Message = "Invalid email or password!" });
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Secret"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(ClaimTypes.Email, user.Email)
            }),
                Expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:ExpiryInMinutes"])),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return Ok(new { Token = tokenString });
        }


        // Delete
        [Authorize(Policy = "SelfOrAdmin")]
        [HttpDelete("deleteUser")]

        public async Task<ActionResult> deleteUser(int userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserId == userId);

            if (user != null) 
            {
                _context.Remove(user);
                await _context.SaveChangesAsync();
                return StatusCode(200, "Sucessfully deleted.");

            }
            return StatusCode(418, "Not Found.");
        }
        
    
    }


}

