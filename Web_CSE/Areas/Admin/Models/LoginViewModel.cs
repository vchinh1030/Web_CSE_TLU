using System.ComponentModel.DataAnnotations;

namespace Web_CSE.Areas.Admin.Models
{
    public class LoginViewModel
    {
        [Key]
        [MaxLength(50)]
        [Required(ErrorMessage = "Vui lòng nhập email")]
        [Display(Name = "Địa chỉ email")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "Vui lòng nhập lại email ")]
        public string Email { get; set; }

        [Display (Name = "Mật khẩu")]
        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        [MaxLength(30, ErrorMessage = "Mật khẩu chỉ được sử dụng 30 kí tự")]
        public string Password { get; set; }
    }
}
