using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AdminPanel
{
    public static class ApiClient
    {
        public static readonly HttpClient httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://localhost:7077/api/")
        };
    }
}
