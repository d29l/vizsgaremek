using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;

namespace AdminPanel
{
    internal class LoginUsers
    {
        public static async Task<string> LoginAsync(string email, string password)
        {
            try
            {
                // Prepare login request
                var loginUser = new { Email = email, Password = password };
                var response = await ApiClient.httpClient.PostAsJsonAsync("users/loginUser", loginUser);

                if (response.IsSuccessStatusCode)
                {
                    // Parse the login response
                    var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
                    if (result != null && !string.IsNullOrEmpty(result.AccessToken))
                    {
                        // Set the authorization header with the token
                        ApiClient.httpClient.DefaultRequestHeaders.Authorization =
                            new AuthenticationHeaderValue("Bearer", result.AccessToken);

                        // Store tokens in the app
                        App.CurrentUserAccessToken = result.AccessToken;
                        App.CurrentUserRefreshToken = result.RefreshToken;

                        // Parse the JWT token
                        var handler = new JwtSecurityTokenHandler();
                        var jwtToken = handler.ReadJwtToken(result.AccessToken);

                        // Extract user ID
                        var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "nameid" || c.Type == "sub");
                        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                        {
                            return "User ID not found in token";
                        }

                        // Extract role
                        var roleClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "role");
                        if (roleClaim == null)
                        {
                            return "Role not found in token";
                        }

                        string role = roleClaim.Value;
                        if (role != "Admin")
                        {
                            return "Access denied: Only admins are allowed";
                        }

                        // If role is "Admin", set CurrentUser.UserId and allow login
                        CurrentUser.UserId = userId;
                        return "Login successful";
                    }
                    return "Unexpected response format";
                }

                // Handle unsuccessful response (e.g., invalid credentials)
                var errorContent = await response.Content.ReadAsStringAsync();
                try
                {
                    var errorResult = await response.Content.ReadFromJsonAsync<ErrorResponse>();
                    return errorResult?.Message ?? $"Error: {response.StatusCode}";
                }
                catch
                {
                    return $"Error: {response.StatusCode}";
                }
            }
            catch (Exception ex)
            {
                return $"Exception: {ex.Message}";
            }
        }
    }

    public class LoginResponse
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }

    public class ErrorResponse
    {
        public string Message { get; set; }
    }
}