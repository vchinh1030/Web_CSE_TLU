using System.Collections.Generic;
using Web_CSE.Models;
using System.ComponentModel.DataAnnotations;

namespace Web_CSE.Areas.Admin.Models
{
   public class UserViewModel
{
    public Account Account { get; set; }

    public UpdateAccountViewModel UpdateAccountViewModel {get; set; }
    public ChangePasswordViewModel ChangePasswordViewModel { get; set; }

}
}
