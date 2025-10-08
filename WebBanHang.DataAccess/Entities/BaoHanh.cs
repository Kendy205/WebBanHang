using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebBanHang.DataAccess.Entities
{
    public class BaoHanh
    {
        [Key]
        public int MaBaoHanh { get; set; }

        [ForeignKey("ChiTietSanPham")]
        public int MaChiTietSP { get; set; }

        public DateTime NgayBatDau { get; set; }

        public DateTime NgayKetThuc { get; set; }

        [StringLength(500)]
        public string NoiDung { get; set; }

        [StringLength(50)]
        public string TrangThai { get; set; } = "Đang bảo hành";

        // Navigation properties
        public virtual ChiTietSanPham ChiTietSanPham { get; set; }
    }
}
