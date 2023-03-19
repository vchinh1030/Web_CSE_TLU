﻿using System;
using System.Collections.Generic;

namespace Web_CSE.Models;

public partial class Post
{
    public int PostId { get; set; }

    public string Title { get; set; }

    public string Describe { get; set; }

    public DateTime Date { get; set; }

    public string Author { get; set; }

    public int AccountId { get; set; }

    public int CatId { get; set; }

    public string Thumb { get; set; }

    public string Alias { get; set; }

    public virtual Account Account { get; set; }

    public virtual Category Cat { get; set; }
}
