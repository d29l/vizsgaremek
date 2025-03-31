﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProjektBackend.Models;

public partial class User
{
    public int UserId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    [DataType(DataType.EmailAddress)]
    public string Email { get; set; } = null!;

    [DataType(DataType.Password)]
    public string Password { get; set; } = null!;

    public string RefreshToken { get; set; } = null!;

    public string? Role { get; set; }

    public DateTime? CreatedAt { get; set; }

    public bool IsVerified { get; set; }

    public bool? IsActive { get; set; }

    public virtual ICollection<Connection> ConnectionReceivers { get; set; } = new List<Connection>();

    public virtual ICollection<Connection> ConnectionRequesters { get; set; } = new List<Connection>();

    public virtual Employer? Employers { get; set; }

    public virtual Employerrequest? Employerrequest { get; set; }

    public virtual ICollection<Message> MessageReceivers { get; set; } = new List<Message>();

    public virtual ICollection<Message> MessageSenders { get; set; } = new List<Message>();

    public virtual ICollection<Profile> Profiles { get; set; } = new List<Profile>();
}
