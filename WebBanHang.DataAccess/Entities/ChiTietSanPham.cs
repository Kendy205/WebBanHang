using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebBanHang.DataAccess.Entities
{
    public class ChiTietSanPham
    {
        [Key]
        public int MaChiTietSP { get; set; }

        [ForeignKey("SanPham")]
        public int MaSP { get; set; }

        [StringLength(100)]
        public string MaDinhDanh { get; set; } // IMEI, Serial...

        [StringLength(100)]
        public string SoSeri { get; set; }

        public DateTime? NgayNhap { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal GiaNhap { get; set; }

        [StringLength(50)]
        public string TrangThai { get; set; } = "Trong kho"; // Trong kho, Đã bán, Đang bảo hành, Lỗi

        [StringLength(100)]
        public string ViTri { get; set; }

        [ForeignKey("HoaDonNhap")]
        public int? MaHDN { get; set; }

        // Navigation properties
        public virtual SanPham SanPham { get; set; }
        public virtual HoaDonNhap HoaDonNhap { get; set; }
        public virtual ChiTietHDB ChiTietHDB { get; set; }
        public virtual ICollection<BaoHanh> BaoHanhs { get; set; }
    }
}
