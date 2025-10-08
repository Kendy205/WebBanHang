using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebBanHang.DataAccess.Entities
{
    public class ChiTietHDN
    {
        [Key]
        public int MaChiTiet { get; set; }

        [ForeignKey("HoaDonNhap")]
        public int MaHDN { get; set; }

        [ForeignKey("SanPham")]
        public int MaSP { get; set; }

        public int SoLuong { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal DonGiaNhap { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal ThanhTien { get; set; }

        // Navigation properties
        public virtual HoaDonNhap HoaDonNhap { get; set; }
        public virtual SanPham SanPham { get; set; }
    }
}
