using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AdminPanel
{
    internal class LoginUsers
    {
        private static readonly HttpClient httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://localhost:7077/api/users/loginUser")
        };

        public static async Task<string> LoginAsync(string email, string password)
        {
            try
            {
                var loginUser = new { Email = email, Password = password };
                string json = JsonSerializer.Serialize(loginUser);

                var request = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync("loginUser", request);


                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var result = await response.Content.ReadAsStringAsync();


                    using (JsonDocument doc = JsonDocument.Parse(result))
                    {
                        JsonElement root = doc.RootElement;

                        if (root.TryGetProperty("token", out JsonElement tokenElement))
                        {
                            string token = tokenElement.GetString();


                            httpClient.DefaultRequestHeaders.Authorization =
                                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                            return "Login successful";
                        }
                    }

                    return "Unexpected response format";
                }
                else
                {
                    return $"Error: {response.StatusCode.ToString()}";
                }
            }
            catch (Exception ex)
            {
                return $"Exception: {ex.Message}";
            }
        }
    }
}