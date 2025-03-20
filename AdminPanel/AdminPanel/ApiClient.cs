using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace AdminPanel
{
    public static class ApiClient
    {
        public static HttpClient httpClient = new HttpClient { BaseAddress = new Uri("https://localhost:7077/api/") };

        public static void SetToken(string token)
        {
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }
    }
}
