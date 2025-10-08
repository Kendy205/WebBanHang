using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebBanHang.DataAccess.Entities
{
    public class ChiTietHDB
    {
        [Key]
        public int MaChiTiet { get; set; }

        [ForeignKey("HoaDonBan")]
        public int MaHDB { get; set; }

        [ForeignKey("SanPham")]
        public int MaSP { get; set; } // Link trực tiếp đến SanPham

        public int SoLuong { get; set; } // Số lượng bán

        [Column(TypeName = "decimal(18,2)")]
        public decimal DonGia { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal GiamGia { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal ThanhTien { get; set; }

        // Navigation properties
        public virtual HoaDonBan HoaDonBan { get; set; }
        public virtual SanPham SanPham { get; set; }
    }
}
