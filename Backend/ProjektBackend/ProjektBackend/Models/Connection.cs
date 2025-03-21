using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ProjektBackend.Models;

public partial class Connection
{
    public int ConnectionId { get; set; }

    public int RequesterId { get; set; }

    public int ReceiverId { get; set; }
    [Required]
    [RegularExpression("^(Accepted|Rejected|Pending)$",
        ErrorMessage = "Status must be 'Accepted', 'Rejected', or 'Pending'.")]
    public string? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    [JsonIgnore]
    public virtual User Receiver { get; set; } = null!;
    [JsonIgnore]
    public virtual User Requester { get; set; } = null!;
}
