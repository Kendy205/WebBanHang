using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebBanHang.DataAccess.Entities
{
    public class SanPham
    {
        [Key]
        [StringLength(20)]
        public string MaSP { get; set; } 
        [Required]
        [StringLength(200)]
        public string TenSP { get; set; }       
        [Required]
        [ForeignKey("LoaiSanPham")]
        public int MaLoai { get; set; }
        [ForeignKey("NhaSanXuat")]
        public string MaNSX { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal DonGiaBan { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal DonGiaNhap { get; set; }
        public int SoLuong { get; set; }
        public string? MoTa { get; set; }
        public string? HinhAnh { get; set; }
        public DateTime NgayCapNhat { get; set; }
        public bool TrangThai { get; set; }
        [ForeignKey("NhaCungCap")]
        public int? MaNCC { get; set; }
        public virtual LoaiSanPham LoaiSP { get; set; }
        public virtual NhaCungCap NhaCungCap { get; set; }
        public virtual NhaSanXuat NhaSanXuat { get; set; }
        public virtual ICollection<ChiTietHDN> ChiTietHDNs { get; set; }
        public virtual ICollection<ChiTietHDB> ChiTietHDBs { get; set; }
    }
}
