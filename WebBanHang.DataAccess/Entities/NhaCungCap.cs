using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebBanHang.DataAccess.Entities
{
    public class NhaCungCap
    {
        [Key]
        [StringLength(20)]
        public string MaNCC { get; set; }

        [Required]
        [StringLength(200)]
        public string TenNCC { get; set; }

        [StringLength(300)]
        public string DiaChi { get; set; }

        [StringLength(20)]
        public string DienThoai { get; set; }

        [StringLength(100)]
        [EmailAddress]
        public string Email { get; set; }

        // Navigation properties
        public virtual ICollection<SanPham> SanPhams { get; set; }
        public virtual ICollection<HoaDonNhap> HoaDonNhaps { get; set; }
    }
}
