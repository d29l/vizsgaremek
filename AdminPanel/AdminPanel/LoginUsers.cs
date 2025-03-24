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
                
                var loginUser = new { Email = email, Password = password };

                
                var response = await ApiClient.httpClient.PostAsJsonAsync("users/loginUser", loginUser);

                if (response.IsSuccessStatusCode)
                {
                    
                    var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
                    if (result != null && !string.IsNullOrEmpty(result.AccessToken))
                    {
                        
                        ApiClient.httpClient.DefaultRequestHeaders.Authorization =
                            new AuthenticationHeaderValue("Bearer", result.AccessToken);

                        
                        App.CurrentUserAccessToken = result.AccessToken;
                        App.CurrentUserRefreshToken = result.RefreshToken;

                        
                        var handler = new JwtSecurityTokenHandler();
                        var jwtToken = handler.ReadJwtToken(result.AccessToken);
                        
                        
                        var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "nameid" || c.Type == "sub");
                        if (userIdClaim != null)
                        {
                            
                            if (int.TryParse(userIdClaim.Value, out int userId))
                            {
                                CurrentUser.UserId = userId;
                            }
                        }

                        return "Login successful";
                    }
                    return "Unexpected response format";
                }

                
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
