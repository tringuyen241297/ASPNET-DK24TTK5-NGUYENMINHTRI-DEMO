using System.ComponentModel.DataAnnotations;

namespace CakeShop.Areas.Admin.Data
{
    public class AdminLoginVM
    {
        [Required(ErrorMessage = "Vui lòng nhập tên đăng nhập (email).")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}


