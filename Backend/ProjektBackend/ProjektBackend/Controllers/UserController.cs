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
using System.Net.Mail;
using System.Net;

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
                var users = await _context.Users
                    .Select(p => new
                    {
                        p.FirstName,
                        p.LastName,
                        p.Role,
                        p.CreatedAt,
                        p.IsVerified,
                        p.IsActive
                    }).ToListAsync();
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
        public async Task<ActionResult<object>> FetchUser(int? userId = null)
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
                    .Where(p => p.UserId == targetUserId)
                    .Select(p => new
                    {
                        p.UserId,
                        p.FirstName,
                        p.LastName,
                        p.Role,
                        p.Email,
                        p.CreatedAt,
                        p.IsVerified,
                        p.IsActive,
                        Employers = p.Employers != null ? new
                        {
                            p.Employers.CompanyAddress,
                            p.Employers.CompanyPhoneNumber,
                            p.Employers.CompanyName,
                            p.Employers.CompanyDescription,
                            p.Employers.Industry,
                            p.Employers.CompanyEmail,
                            p.Employers.CompanyWebsite,
                        } : null
                    })
                    .FirstOrDefaultAsync();

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
                    RefreshToken = string.Empty,
                    IsVerified = false,
                    IsActive = true,
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
                    new Claim("verified", user.IsVerified.ToString()),
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

        [HttpPost("sendVerificationCode")]
        public async Task<ActionResult> SendVerificationCode(string email)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

                if (user == null)
                {
                    return StatusCode(404, "User not found.");
                }

                if (user.IsVerified)
                {
                    return StatusCode(400, "User is already verified.");
                }

                string verificationCode = GenerateVerificationCode();

                HttpContext.Session.SetString($"VerificationCode_{email}", verificationCode);
                HttpContext.Session.SetString($"VerificationCodeExpiry_{email}",
                    DateTime.UtcNow.AddMinutes(15).ToString("o"));

                await SendVerificationEmail(email, verificationCode);

                return StatusCode(200, "Verification code sent.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while sending verification code.");
            }
        }

        [HttpPost("verifyEmail")]
        public async Task<ActionResult> VerifyEmail(string email, string verificationCode)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

                if (user == null)
                {
                    return StatusCode(404, "User not found.");
                }

                if (user.IsVerified)
                {
                    return StatusCode(400, "User is already verified.");
                }

                string storedCode = HttpContext.Session.GetString($"VerificationCode_{email}");
                string expiryString = HttpContext.Session.GetString($"VerificationCodeExpiry_{email}");

                if (string.IsNullOrEmpty(storedCode) || string.IsNullOrEmpty(expiryString))
                {
                    return StatusCode(400, "No verification code found or it has expired. Please request a new one.");
                }

                DateTime expiry = DateTime.Parse(expiryString);
                if (DateTime.UtcNow > expiry)
                {
                    return StatusCode(400, "Verification code has expired. Please request a new one.");
                }

                if (storedCode != verificationCode)
                {
                    return StatusCode(400, "Invalid verification code.");
                }

                //Commented for testing purposes
                //user.IsVerified = true;
                await _context.SaveChangesAsync();

                HttpContext.Session.Remove($"VerificationCode_{email}");
                HttpContext.Session.Remove($"VerificationCodeExpiry_{email}");

                return StatusCode(200, "Email verified successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while verifying email.");
            }
        }

        private async Task SendVerificationEmail(string email, string verificationCode)
        {
            var smtpSettings = _configuration.GetSection("SmtpSettings");

            using var message = new MailMessage();
            message.From = new MailAddress("noreply@jobplatform.hu");
            message.To.Add(email);
            message.Subject = "Email Verification";
            var emailBody = @"<!DOCTYPE html>
            <html lang=""en"">
            <head>
                <meta charset=""UTF-8"">
                <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
                <meta http-equiv=""X-UA-Compatible"" content=""ie=edge"">
                <title>Your Job Platform Verification Code</title>
                <style type=""text/css"">
                    body, table, td, a { -webkit-text-size-adjust: 100%; -ms-text-size-adjust: 100%; }
                    table, td { mso-table-lspace: 0pt; mso-table-rspace: 0pt; }
                    img { -ms-interpolation-mode: bicubic; border: 0; height: auto; line-height: 100%; outline: none; text-decoration: none; }
                    table { border-collapse: collapse !important; }
                    body { height: 100% !important; margin: 0 !important; padding: 0 !important; width: 100% !important; background-color: #1e2030; }

                    .email-container {
                        max-width: 600px;
                        margin: 0 auto;
                    }
                    .content-block {
                        background-color: #363a4f;
                        border-radius: 8px;
                        padding: 30px 40px;
                        color: #cad3f5;
                        font-family: Arial, Helvetica, sans-serif;
                        font-size: 16px;
                        line-height: 1.6;
                    }
                    .code-display-container {
                        background-color: #494d64;
                        border-radius: 4px;
                        padding: 15px 20px;
                        text-align: center;
                        margin: 25px 0;
                    }
                    .verification-code {
                        font-size: 32px;
                        font-weight: bold;
                        color: #b7bdf8;
                        letter-spacing: 5px;
                        font-family: 'Courier New', Courier, monospace;
                        display: inline-block;
                    }
                    .expiry-info {
                        font-size: 14px;
                        color: #b8c0e0;
                        text-align: center;
                        margin-top: 10px;
                    }
                    .footer-text {
                        font-size: 12px;
                        color: #a5adcb;
                        text-align: center;
                        padding-top: 20px;
                        font-family: Arial, Helvetica, sans-serif;
                    }
                    .footer-text a {
                        color: #b7bdf8;
                        text-decoration: none;
                    }
                    h1 {
                        color: #cad3f5;
                        font-size: 24px;
                        margin-bottom: 20px;
                        text-align: center;
                    }
                    p {
                        margin-bottom: 15px;
                    }

                    @media screen and (max-width: 600px) {
                        .content-block {
                            padding: 20px !important;
                        }
                        .verification-code {
                            font-size: 28px !important;
                            letter-spacing: 3px !important;
                        }
                        h1 {
                            font-size: 20px !important;
                        }
                    }
                </style>
            </head>
            <body style=""margin: 0 !important; padding: 0 !important; background-color: #1e2030;"">
                <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""background-color: #1e2030;"">
                    <tr>
                        <td align=""center"" valign=""top"" style=""padding: 20px 10px;"">
                            <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" class=""email-container"">
                                <tr>
                                    <td align=""center"" valign=""top"" class=""content-block"">

                                        <h1 style=""color: #b7bdf8; font-size: 24px; margin-bottom: 5px; text-align: center;"">Job Platform</h1>
                                        <h1>Your Verification Code</h1>

                                        <p style=""text-align: center;"">Please use the following code to complete your verification. This code is required to ensure your account's security.</p>

                                        <div class=""code-display-container"">
                                            <span class=""verification-code"">
                                                {{CODE}}
                                            </span>
                                            <div class=""expiry-info"">
                                                This code expires in 10 minutes.
                                            </div>
                                        </div>

                                        <p style=""text-align: center;"">Enter this code in the verification field on the Job Platform website or app.</p>
                                        <p style=""text-align: center; font-size: 14px; color: #b8c0e0;"">If you did not request this code, please ignore this email or contact our support if you suspect suspicious activity.</p>

                                    </td>
                                </tr>
                                <tr>
                                    <td align=""center"" class=""footer-text"">
                                        You received this email because a verification attempt was made for your account.
                                        <br><br>
                                        &copy; 2025 Job Platform. All rights reserved.
                                        <br>
                                        <a href=""https://jobplatform.hu"">jobplatform.hu</a>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </body>
</html>";
            emailBody = emailBody.Replace("{{CODE}}", verificationCode);
            message.Body = emailBody;
            message.IsBodyHtml = true;

            using var client = new SmtpClient(smtpSettings["Server"])
            {
                Port = int.Parse(smtpSettings["Port"]),
                Credentials = new NetworkCredential(smtpSettings["User"], smtpSettings["Password"]),
                EnableSsl = bool.Parse(smtpSettings["EnableSsl"])
            };

            await client.SendMailAsync(message);
        }


        private string GenerateVerificationCode()
        {
            Random random = new Random();
            return random.Next(100000, 999999).ToString();
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
