using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebBanHang.DataAccess.Entities
{
    public class KhachHang
    {
        [Key]
        public int MaKH { get; set; }
        [Required]
        [StringLength(200)]
        public string TenKH { get; set; }

        [StringLength(300)]
        public string DiaChi { get; set; }

        [StringLength(20)]
        public string DienThoai { get; set; }

        [StringLength(100)]
        [EmailAddress]
        public string Email { get; set; }

        [DataType(DataType.Date)]
        public DateTime? NgaySinh { get; set; }

        public int DiemTichLuy { get; set; } = 0;

        // Navigation properties
        public virtual ICollection<HoaDonBan> HoaDonBans { get; set; }
    }
}
