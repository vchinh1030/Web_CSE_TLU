using System.Collections.Generic;
using Web_CSE.Models;

namespace Web_CSE.Areas.Admin.Models
{
    public class DashboardViewModel
    {
        public List<Account> Accounts { get; set; }
        public NotificationViewModel Notification { get; set; }

    }
}
