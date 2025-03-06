using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

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
                    var result = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
                    if (result.TryGetValue("token", out var token))
                    {
                        // Set the Authorization header
                        ApiClient.httpClient.DefaultRequestHeaders.Authorization =
                            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                        return "Login successful";
                    }
                    return "Unexpected response format";
                }
                return $"Error: {response.StatusCode}";
            }
            catch (Exception ex)
            {
                return $"Exception: {ex.Message}";
            }
        }
    }
}
