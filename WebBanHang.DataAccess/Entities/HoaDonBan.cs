using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebBanHang.DataAccess.Entities
{
    public class HoaDonBan
    {
        [Key]
        public int MaHDB { get; set; }

        [Required]
        public DateTime NgayBan { get; set; } = DateTime.Now;

        [ForeignKey("KhachHang")]
        public int? MaKH { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TongTien { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal GiamGia { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal ThanhTien { get; set; }

        [StringLength(500)]
        public string GhiChu { get; set; }

        //[ForeignKey("KhuyenMai")]
        //public int? MaKM { get; set; }

        // Navigation properties
        public virtual KhachHang KhachHang { get; set; }
        //public virtual KhuyenMai KhuyenMai { get; set; }
        public virtual ICollection<ChiTietHDB> ChiTietHDBs { get; set; }
    }
}
