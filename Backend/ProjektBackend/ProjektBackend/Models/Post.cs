using System;
using System.Collections.Generic;

namespace ProjektBackend.Models;

public partial class Post
{
    public int PostId { get; set; }

    public int UserId { get; set; }

    public string Title { get; set; } = null!;

    public string Content { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public int? Likes { get; set; }
}
