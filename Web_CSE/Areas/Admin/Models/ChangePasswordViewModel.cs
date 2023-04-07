using System.Collections.Generic;
using Web_CSE.Models;
using System.ComponentModel.DataAnnotations;

namespace Web_CSE.Areas.Admin.Models
{
public class ChangePasswordViewModel
{
    [Required(ErrorMessage = "Vui lòng nhập mật khẩu cũ")]
    [DataType(DataType.Password)]
    [Display(Name = "Mật khẩu cũ")]
    public string OldPassword { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập mật khẩu mới")]
    [StringLength(100, ErrorMessage = "Mật khẩu mới phải dài ít nhất {2} ký tự.", MinimumLength = 6)]
    [DataType(DataType.Password)]
    [Display(Name = "Mật khẩu mới")]
    public string NewPassword { get; set; }

    [DataType(DataType.Password)]
    [Display(Name = "Xác nhận mật khẩu mới")]
    [Compare("NewPassword", ErrorMessage = "Mật khẩu xác nhận không khớp.")]
    public string ConfirmPassword { get; set; }
}
}