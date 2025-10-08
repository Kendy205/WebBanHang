using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebBanHang.DataAccess.Entities
{
    public class HoaDonNhap
    {
        [Key]
        public int MaHDN { get; set; }

        [Required]
        public DateTime NgayNhap { get; set; } = DateTime.Now;

        [ForeignKey("NhaCungCap")]
        public string? MaNCC { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TongTien { get; set; }

        [StringLength(500)]
        public string GhiChu { get; set; }

        public bool TrangThaiThanhToan { get; set; } = false;

        // Navigation properties
        public virtual NhaCungCap NhaCungCap { get; set; }
        public virtual ICollection<ChiTietHDN> ChiTietHDNs { get; set; }
        //public virtual ICollection<ChiTietSanPham> ChiTietSanPhams { get; set;}
    }
}
