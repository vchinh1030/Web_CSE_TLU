using System;
using System.Collections.Generic;

namespace Web_CSE.Models;

public partial class Role
{
    public int RoleId { get; set; }

    public string RoleName { get; set; }

    public string RoleDescribtion { get; set; }

    public virtual ICollection<Account> Accounts { get; } = new List<Account>();
}
