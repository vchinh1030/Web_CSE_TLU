using System.Collections.Generic;
using Web_CSE.Models;
using System.ComponentModel.DataAnnotations;

namespace Web_CSE.Areas.Admin.Models
{
    public class UpdateAccountViewModel
    {
        [Required(ErrorMessage = "Hãy nhập họ tên của bạn")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Hãy nhập số điện thoại của bạn")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        public string Phone { get; set; }

        public Account Account { get; set; }
    }
}
