using System;
using System.Collections.Generic;

namespace ProjektBackend.Models;

public partial class Connection
{
    public int ConnectionId { get; set; }

    public int RequesterId { get; set; }

    public int ReceiverId { get; set; }

    public string? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual User Receiver { get; set; } = null!;

    public virtual User Requester { get; set; } = null!;
}
