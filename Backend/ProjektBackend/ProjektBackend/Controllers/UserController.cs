using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ProjektBackend.Models;
using ProjektBackend.PasswordHandler;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

#pragma warning disable CS8604
#pragma warning disable CS8602
#pragma warning disable CS8600
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

        string letterOnlyPattern = @"^[a-zA-ZáéíóöőúüűÁÉÍÓÖŐÚÜŰ]+$";

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
        [HttpGet("fetchUser")]
        public async Task<ActionResult<User>> FetchUser(int? userId = null)
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

                var user = await _context.Users
                    .Include(p => p.Employers)
                    .FirstOrDefaultAsync(p => p.UserId == targetUserId);

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

                if (newUser != null)
                {
                    if (!newUser.Email.Contains("@"))
                    {
                        return StatusCode(418, "Invalid Email address.");
                    }
                    if (!Regex.IsMatch(registerUserDto.FirstName, letterOnlyPattern))
                    {
                        return StatusCode(418, "Invalid First Name. Only letters are allowed.");
                    }

                    if (!Regex.IsMatch(registerUserDto.LastName, letterOnlyPattern))
                    {
                        return StatusCode(418, "Invalid Last Name. Only letters are allowed.");
                    }

                    if (!PasswordPolicy.IsValid(registerUserDto.Password))
                    {
                        return StatusCode(400, "Password does not meet security requirements.");
                    }


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
                    new Claim("status", user.IsActive.ToString()),
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

        [HttpPost("logout")]
        public async Task<ActionResult> Logout([FromBody] LogoutRequestDto logoutRequestDto)
        {
            try
            {
                if (string.IsNullOrEmpty(logoutRequestDto.RefreshToken))
                {
                    return StatusCode(400, "Refresh token is required.");
                }

                var user = await _context.Users.FirstOrDefaultAsync(u => u.RefreshToken == logoutRequestDto.RefreshToken);

                if (user == null)
                {
                    return StatusCode(404, "Invalid refresh token.");
                }

                user.RefreshToken = string.Empty;
                _context.Update(user);
                await _context.SaveChangesAsync();

                return StatusCode(200, "Logout successful.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred during logout.");
            }
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpPost("revokeToken")]
        public async Task<ActionResult> RevokeToken([FromBody] RevokeTokenRequestDto revokeTokenRequestDto, int? userId = null)
        {
            try
            {
                if (string.IsNullOrEmpty(revokeTokenRequestDto.RefreshToken) && !userId.HasValue)
                {
                    return StatusCode(400, "Either refresh token or user ID is required.");
                }

                User user = null;

                if (!string.IsNullOrEmpty(revokeTokenRequestDto.RefreshToken))
                {
                    user = await _context.Users.FirstOrDefaultAsync(u => u.RefreshToken == revokeTokenRequestDto.RefreshToken);
                }
                else if (userId.HasValue)
                {
                    user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId.Value);
                }

                if (user == null)
                {
                    return StatusCode(404, "User not found.");
                }

                user.RefreshToken = string.Empty;
                _context.Update(user);
                await _context.SaveChangesAsync();

                return StatusCode(200, "Token successfully revoked.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while revoking the token.");
            }
        }


        [Authorize(Policy = "AdminOnly")]
        [HttpPut("updateUserRole/{userId}")]
        public async Task<ActionResult> UpdateUserRole(int userId, string Role)
        {
            try
            {
                var existingUser = await _context.Users.FirstOrDefaultAsync(x => x.UserId == userId);

                if (existingUser != null)
                {
                    if (Role != "Employee" && Role != "Employer" && Role != "Admin")
                    {
                        return StatusCode(403, "You are not allowed to give custom Roles");
                    }
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
        [HttpPut("updateUser")]
        public async Task<ActionResult> UpdateUser(UpdateUserDto updateUserDto, int? userId = null)
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

                var existingUser = await _context.Users.FirstOrDefaultAsync(x => x.UserId == targetUserId);

                if (existingUser != null)
                {
                    existingUser.FirstName = updateUserDto.FirstName ?? existingUser.FirstName;
                    existingUser.LastName = updateUserDto.LastName ?? existingUser.LastName;
                    existingUser.Email = updateUserDto.Email ?? existingUser.Email;
                    if (!updateUserDto.Email.Contains("@"))
                    {
                        return StatusCode(418, "Invalid Email address.");
                    }
                    if (!Regex.IsMatch(updateUserDto.FirstName, letterOnlyPattern))
                    {
                        return StatusCode(418, "Invalid First Name. Only letters are allowed.");
                    }

                    if (!Regex.IsMatch(updateUserDto.LastName, letterOnlyPattern))
                    {
                        return StatusCode(418, "Invalid Last Name. Only letters are allowed.");
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

        [Authorize("SelfOrAdmin")]
        [HttpPut("changePassword")]
        public async Task<ActionResult> ChangePassword(ChangePasswordDto changePasswordDto, int? userId = null)
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

                var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == targetUserId);

                if (user == null)
                {
                    return StatusCode(404, "User not found.");
                }

                bool skipCurrentPasswordCheck = userId.HasValue && isAdmin && targetUserId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

                if (!skipCurrentPasswordCheck)
                {
                    byte[] hashBytes = Convert.FromBase64String(user.Password);

                    byte[] salt = new byte[16];
                    Array.Copy(hashBytes, 0, salt, 0, 16);

                    var pbkdf2 = new Rfc2898DeriveBytes(changePasswordDto.CurrentPassword, salt, 100000, HashAlgorithmName.SHA256);
                    byte[] currentHash = pbkdf2.GetBytes(32);

                    bool isCurrentPasswordValid = true;
                    for (int i = 0; i < 32; i++)
                    {
                        if (hashBytes[i + 16] != currentHash[i])
                        {
                            isCurrentPasswordValid = false;
                            break;
                        }
                    }

                    if (!isCurrentPasswordValid)
                    {
                        return StatusCode(401, "Current password is incorrect.");
                    }
                }

                if (!PasswordPolicy.IsValid(changePasswordDto.NewPassword))
                {
                    return StatusCode(400, "New password does not meet security requirements.");
                }

                byte[] newSalt = new byte[16];
                using (var rng = RandomNumberGenerator.Create())
                {
                    rng.GetBytes(newSalt);
                }

                var newPbkdf2 = new Rfc2898DeriveBytes(changePasswordDto.NewPassword, newSalt, 100000, HashAlgorithmName.SHA256);
                byte[] newHash = newPbkdf2.GetBytes(32);

                byte[] newHashBytes = new byte[48];
                Array.Copy(newSalt, 0, newHashBytes, 0, 16);
                Array.Copy(newHash, 0, newHashBytes, 16, 32);

                user.Password = Convert.ToBase64String(newHashBytes);

                _context.Update(user);
                await _context.SaveChangesAsync();

                return StatusCode(200, "Password changed successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while changing the password.");
            }
        }

        [Authorize(Policy = "SelfOrAdmin")]
        [HttpDelete("deleteUser")]
        public async Task<ActionResult> DeleteUser([FromBody] DeleteUserRequestDto deleteRequest, int? userId = null)
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
                    {
                        return Unauthorized("User ID not found in token.");
                    }
                    targetUserId = int.Parse(userIdClaim.Value);
                }

                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.UserId == targetUserId);

                if (user == null)
                {
                    return StatusCode(404, "User not found.");
                }

                if (!isAdmin || (isAdmin && targetUserId == int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)))
                {
                    if (string.IsNullOrEmpty(deleteRequest?.Password))
                    {
                        return StatusCode(400, "Password is required for account deletion.");
                    }

                    byte[] hashBytes = Convert.FromBase64String(user.Password);
                    byte[] salt = new byte[16];
                    Array.Copy(hashBytes, 0, salt, 0, 16);

                    var pbkdf2 = new Rfc2898DeriveBytes(deleteRequest.Password, salt, 100000, HashAlgorithmName.SHA256);
                    byte[] inputHash = pbkdf2.GetBytes(32);

                    bool isPasswordValid = true;
                    for (int i = 0; i < 32; i++)
                    {
                        if (hashBytes[i + 16] != inputHash[i])
                        {
                            isPasswordValid = false;
                            break;
                        }
                    }

                    if (!isPasswordValid)
                    {
                        return StatusCode(401, "Incorrect password.");
                    }
                }
                    _context.Users.Remove(user);
                    await _context.SaveChangesAsync();
                    return StatusCode(200, "User successfully deleted.");
                
            }
            catch (DbUpdateException dbEx)
            {
                return StatusCode(409, "Unable to delete user due to database constraints.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while deleting the user.");
            }
        }
    }
}
