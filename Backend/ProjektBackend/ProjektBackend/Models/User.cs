using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ProjektBackend.Models;

public partial class User
{
    public int UserId { get; set; }

    [Required]
    public string FirstName { get; set; } = null!;
    [Required]
    public string LastName { get; set; } = null!;
    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;
    [Required]
    [DataType(DataType.Password)]
    [JsonIgnore]
    public string Password { get; set; } = null!;

    public string? Role { get; set; }

    public DateTime? CreatedAt { get; set; }

    public bool? IsActive { get; set; }
    [JsonIgnore]
    public virtual ICollection<Connection> ConnectionReceivers { get; set; } = new List<Connection>();
    [JsonIgnore]
    public virtual ICollection<Connection> ConnectionRequesters { get; set; } = new List<Connection>();
    [JsonIgnore]
    public virtual ICollection<Employer> Employers { get; set; } = new List<Employer>();
    [JsonIgnore]
    public virtual ICollection<Message> MessageReceivers { get; set; } = new List<Message>();
    [JsonIgnore]
    public virtual ICollection<Message> MessageSenders { get; set; } = new List<Message>();
    [JsonIgnore]
    public virtual ICollection<Profile> Profiles { get; set; } = new List<Profile>();
}
