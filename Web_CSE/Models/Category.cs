using System;
using System.Collections.Generic;

namespace Web_CSE.Models;

public partial class Category
{
    public int CatId { get; set; }

    public string CatName { get; set; }

    public int? Ordering { get; set; }

    public int? Parent { get; set; }

    public int? Levels { get; set; }

    public virtual ICollection<Post> Posts { get; } = new List<Post>();
}
