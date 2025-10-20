using System.ComponentModel.DataAnnotations;

namespace WebBanHang.DTOs
{
    public class ApplicationUserDTO
    {
        [Required(ErrorMessage = "Vui lòng nhập họ và tên.")]
        [Display(Name = "Họ và tên")]
        [StringLength(100, ErrorMessage = "{0} phải dài từ {2} đến {1} ký tự.", MinimumLength = 6)]
        public string FulName { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập Email.")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng.")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Phone(ErrorMessage = "Số điện thoại không đúng định dạng.")]
        [Display(Name = "Số điện thoại")]
        public string? PhoneNumber { get; set; } // Dấu ? cho phép trường này không bắt buộc

        [Display(Name = "Địa chỉ")]
        public string? Address { get; set; } // Dấu ? cho phép trường này không bắt buộc

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu.")]
        [StringLength(100, ErrorMessage = "{0} phải dài từ {2} đến {1} ký tự.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        public string Password { get; set; }

        //[DataType(DataType.Password)]
        //[Display(Name = "Xác nhận mật khẩu")]
        //[Compare("Password", ErrorMessage = "Mật khẩu và mật khẩu xác nhận không khớp.")]
        //public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn một quyền.")]
        [Display(Name = "Quyền")]
        public string Role { get; set; }
        
    }
}
