using System;
using System.Collections.Generic;

namespace ProjektBackend.Models;

public partial class Employer
{
    public int EmployerId { get; set; }

    public int UserId { get; set; }

    public string CompanyName { get; set; } = null!;

    public string? CompanyAddress { get; set; }

    public string CompanyEmail { get; set; } = null!;

    public int CompanyPhoneNumber { get; set; }

    public string? Industry { get; set; }

    public string? CompanyWebsite { get; set; }

    public string? CompanyDescription { get; set; }

    public int? EstablishedYear { get; set; }

    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();

    public virtual User User { get; set; } = null!;
}
