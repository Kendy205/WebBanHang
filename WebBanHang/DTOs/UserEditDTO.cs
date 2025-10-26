using System.ComponentModel.DataAnnotations;

namespace WebBanHang.DTOs
{
    public class UserEditDTO
    {
        // Phải có ID để xác định người dùng đang sửa
        public string Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập họ và tên.")]
        [Display(Name = "Họ và tên")]
        [StringLength(100, ErrorMessage = "{0} phải dài từ {2} đến {1} ký tự.", MinimumLength = 6)]
        public string FulName { get; set; }

        // Email thường là Readonly và không được sửa qua form này
        // Chúng ta vẫn cần nó để hiển thị, nhưng không cần [Required] nếu không sửa
        public string Email { get; set; }

        [Phone(ErrorMessage = "Số điện thoại không đúng định dạng.")]
        [Display(Name = "Số điện thoại")]
        public string? PhoneNumber { get; set; }

        [Display(Name = "Địa chỉ")]
        public string? Address { get; set; }

        [Display(Name = "Quyền")]
        public string Role { get; set; }

        // Trường cho ảnh hiện tại (được gửi qua hidden field)
        public string? ImgUrl { get; set; }

        // Trường cho file ảnh mới được upload
        [Display(Name = "Ảnh mới")]
        public IFormFile? NewAvatarFile { get; set; }
    }
}
