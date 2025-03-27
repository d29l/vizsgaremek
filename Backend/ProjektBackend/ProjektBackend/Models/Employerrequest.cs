using System;
using System.Collections.Generic;

namespace ProjektBackend.Models;

public partial class Employerrequest
{
    public int ApplicantId { get; set; }

    public int UserId { get; set; }

    public string CompanyName { get; set; } = null!;

    public string CompanyAddress { get; set; } = null!;

    public string CompanyEmail { get; set; } = null!;

    public int? CompanyPhoneNumber { get; set; } = null!;

    public string Industry { get; set; } = null!;

    public string CompanyWebsite { get; set; } = null!;

    public string CompanyDescription { get; set; } = null!;

    public int EstabilishedYear { get; set; }

    public virtual User User { get; set; } = null!;
}
