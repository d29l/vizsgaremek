using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AdminPanel
{
    public class User
    {
        [JsonPropertyName("userId")]
        public int UserId { get; set; }
        [JsonPropertyName("firstName")]
        public string FirstName { get; set; }

        [JsonPropertyName("lastName")]
        public string LastName { get; set; }

        [JsonPropertyName("password")]
        public string Password { get; set; }

        [JsonPropertyName("refreshToken")]
        public string RefreshToken { get; set; } 

        [JsonPropertyName("role")]
        public string Role { get; set; } 

        [JsonPropertyName("createdAt")]
        public DateTime? CreatedAt { get; set; } 

        [JsonPropertyName("isVerified")]
        public bool IsVerified { get; set; }

        [JsonPropertyName("isActive")]
        public bool? IsActive { get; set; } 
    }

}