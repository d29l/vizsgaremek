using Microsoft.AspNetCore.Authorization;
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

#pragma warning disable CS8604
#pragma warning disable CS0168

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

        char[] specialCharsAndNumbers =
        {
            '0', '1', '2', '3', '4', '5', '6', '7', '8', '9',

            '!', '"', '#', '$', '%', '&', '\'', '(', ')', '*', '+',
            ',', '-', '.', '/', ':', ';', '<', '=', '>', '?', '@',
            '[', '\\', ']', '^', '_', '`', '{', '|', '}', '~',

            '¡', '¿', '¢', '£', '¤', '¥', '§', '©', '®', '°', '±', '×', '÷', 'µ', '¶', '·',
            '•', '∞', '≈', '√', '∑', '∏', '≠', '≤', '≥', '∂', '∫', '∩', '∪',

            '(', ')', '[', ']', '{', '}', '<', '>', '⌈', '⌉', '⌊', '⌋', '⟨', '⟩',

            '$', '€', '£', '¥', '₣', '₤', '₹', '₱', '₿', '₲', '₴', '₽',

            '⁰', '¹', '²', '³', '⁴', '⁵', '⁶', '⁷', '⁸', '⁹', '⁺', '−', '÷', '≡', '∼', '≈',
            '≠', '≡', '∀', '∃', '⊂', '⊆', '⊇', '⊕', '⊗', '⊥', '∠', '∇',

            'α', 'β', 'γ', 'δ', 'ε', 'ζ', 'η', 'θ', 'ι', 'κ', 'λ', 'μ', 'ν', 'ξ', 'ο', 'π',
            'ρ', 'σ', 'τ', 'υ', 'φ', 'χ', 'ψ', 'ω', 'Α', 'Β', 'Γ', 'Δ', 'Ε', 'Ζ', 'Η', 'Θ',
            'Ι', 'Κ', 'Λ', 'Μ', 'Ν', 'Ξ', 'Ο', 'Π', 'Ρ', 'Σ', 'Τ', 'Υ', 'Φ', 'Χ', 'Ψ', 'Ω',

            '∫', '∑', '∏', '∞', '⊂', '⊆', '⊇', '⊕', '⊗', '≠', '≡', '≅', '∼', '≈',

            '♥', '♦', '♣', '♠', '♪', '♫', '☀', '☂', '☃', '☠', '✈', '✉', '☃', '⚡', '☻',

            '←', '↑', '→', '↓', '↔', '↕', '↗', '↖', '⇐', '⇑', '⇒', '⇓', '⇔',

            '∀', '∃', '∧', '∨', '∩', '∪', '∈', '∉', '⊂', '⊄', '⊆', '⊇', '⊕', '⊗', '⊥',
            '≠', '≡', '≤', '≥',

            ' ', '\t', '\n', '\r', '\b', '\f', '\v', '\a',

            '─', '┄', '┅', '┈', '┉', '━', '┛', '┓', '┃', '┏', '┛', '┣', '┫', '┳', '┻', '┳',
            '┃', '╳', '■', '□', '▣', '▢', '▯', '●', '○'
        };

        [Authorize(Policy = "AdminOnly")]
        [HttpGet("fetchUsers")]
        public async Task<ActionResult<User>> FetchUsers()
        {
            try
            {
                var users = await _context.Users.ToListAsync();
                if (users != null && users.Any())
                {
                    return StatusCode(200, users);
                }
                return StatusCode(404, "There are currently no Users.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while fetching users.");
            }
        }

        [Authorize(Policy = "SelfOrAdmin")]
        [HttpGet("fetchUser/{UserId}")]
        public async Task<ActionResult<User>> FetchUser(int UserId)
        {
            try
            {
                var user = await _context.Users
                .Include(p => p.Employers)
                .FirstOrDefaultAsync(p => p.UserId == UserId);

                if (user != null)
                {
                    return StatusCode(200, user);
                }
                return StatusCode(404, "No User can be found with this Id.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while fetching the user.");
            }
        }

        [HttpPost("registerUser")]
        public async Task<ActionResult<User>> RegisterUser(RegisterUserDto registerUserDto)
        {
            try
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
                    Password = Convert.ToBase64String(hashBytes),
                    Role = "Employee",
                    RefreshToken = string.Empty
                };

                if (!newUser.Email.Contains("@"))
                {
                    return StatusCode(418, "Invalid Email address.");
                }
                foreach (var item in specialCharsAndNumbers)
                {
                    if (newUser.FirstName.Contains(item))
                    {
                        return StatusCode(418, "Invalid First Name.");
                    }
                    if (newUser.LastName.Contains(item))
                    {
                        return StatusCode(418, "Invalid Last Name.");
                    }
                }

                if (newUser != null)
                {
                    _context.Add(newUser);
                    await _context.SaveChangesAsync();
                    return StatusCode(201, "User created successfully.");
                }

                return StatusCode(400, "Invalid data.");
            }
            catch (DbUpdateException dbEx)
            {
                return StatusCode(409, "Unable to register user. The email may already be in use.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while registering the user.");
            }
        }

        [HttpPost("loginUser")]
        public async Task<ActionResult<string>> LoginUser(LoginUserDto loginUserDto)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == loginUserDto.Email);

                if (user == null)
                {
                    return StatusCode(401, "Invalid email or password.");
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
                    return StatusCode(401, "Invalid email or password.");
                }

                var accessToken = GenerateAccessToken(user);

                var refreshToken = GenerateRefreshToken();

                user.RefreshToken = refreshToken;
                _context.Update(user);
                await _context.SaveChangesAsync();

                return StatusCode(200, new
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred during login.");
            }
        }

        [HttpPost("refreshToken")]
        public async Task<ActionResult<string>> RefreshToken([FromBody] RefreshTokenDto refreshTokenDto)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshTokenDto.RefreshToken);

                if (user == null)
                {
                    return StatusCode(401, "Invalid RefreshToken.");
                }

                var accessToken = GenerateAccessToken(user);

                var newRefreshToken = GenerateRefreshToken();

                user.RefreshToken = newRefreshToken;
                _context.Update(user);
                await _context.SaveChangesAsync();

                return StatusCode(200, new
                {
                    AccessToken = accessToken,
                    RefreshToken = newRefreshToken
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while refreshing the token.");
            }
        }

        private string GenerateAccessToken(User user)
        {
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
            return tokenHandler.WriteToken(token);
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpPut("updateUserRole/{UserId}")]
        public async Task<ActionResult> UpdateUserRole(int UserId, string Role)
        {
            try
            {
                var existingUser = await _context.Users.FirstOrDefaultAsync(x => x.UserId == UserId);

                if (existingUser != null)
                {
                    existingUser.Role = Role;
                    _context.Update(existingUser);
                    await _context.SaveChangesAsync();
                    return StatusCode(200, "User Role updated.");
                }
                return StatusCode(404, "No User can be found with this Id.");
            }
            catch (DbUpdateException dbEx)
            {
                return StatusCode(409, "Unable to update user role.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while updating the user role.");
            }
        }

        [Authorize(Policy = "SelfOrAdmin")]
        [HttpPut("updateUser/{UserId}")]
        public async Task<ActionResult> UpdateUser(int UserId, UpdateUserDto updateUserDto)
        {
            try
            {
                var existingUser = await _context.Users.FirstOrDefaultAsync(x => x.UserId == UserId);

                if (existingUser != null)
                {
                    existingUser.FirstName = updateUserDto.FirstName;
                    existingUser.LastName = updateUserDto.LastName;
                    existingUser.Email = updateUserDto.Email;
                    if (!updateUserDto.Email.Contains("@"))
                    {
                        return StatusCode(418, "Invalid Email address.");
                    }
                    foreach (var item in specialCharsAndNumbers)
                    {
                        if (updateUserDto.FirstName.Contains(item))
                        {
                            return StatusCode(418, "Invalid First Name.");
                        }
                        if (updateUserDto.LastName.Contains(item))
                        {
                            return StatusCode(418, "Invalid Last Name.");
                        }
                    }

                    _context.Update(existingUser);
                    await _context.SaveChangesAsync();
                    return StatusCode(200, "User updated.");
                }
                return StatusCode(404, "No User can be found with this Id.");
            }
            catch (DbUpdateException dbEx)
            {
                return StatusCode(409, "Unable to update user. The email may already be in use.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while updating the user.");
            }
        }

        [Authorize(Policy = "SelfOrAdmin")]
        [HttpDelete("deleteUser/{UserId}")]
        public async Task<ActionResult> DeleteUser(int UserId)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(x => x.UserId == UserId);

                if (user != null)
                {
                    _context.Remove(user);
                    await _context.SaveChangesAsync();
                    return StatusCode(200, "User successfully deleted.");
                }
                return StatusCode(404, "No User can be found with this Id.");
            }
            catch (DbUpdateException dbEx)
            {
                return StatusCode(409, "Unable to delete user due to related records.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while deleting the user.");
            }
        }
    }
}