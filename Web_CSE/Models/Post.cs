using System;
using System.Collections.Generic;

namespace Web_CSE.Models;

public partial class Post
{
    public int PostId { get; set; }

    public string Title { get; set; }

    public string Contents { get; set; }

    public string Thumb { get; set; }

    public string Alias { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int? AccountId { get; set; }

    public string ShortContent { get; set; }

    public int? CatId { get; set; }

    public virtual Account Account { get; set; }

    public virtual Category Cat { get; set; }
}
