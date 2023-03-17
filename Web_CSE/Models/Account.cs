using System;
using System.Collections.Generic;

namespace Web_CSE.Models;

public partial class Account
{
    public int AccountId { get; set; }

    public string UserName { get; set; }

    public string Password { get; set; }

    public int? Phone { get; set; }

    public string Email { get; set; }

    public byte[] Image { get; set; }

    public string FullName { get; set; }

    public string Describtion { get; set; }

    public int RoleId { get; set; }

    public virtual ICollection<Post> Posts { get; } = new List<Post>();

    public virtual Role Role { get; set; }
}
