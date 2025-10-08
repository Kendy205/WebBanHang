using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebBanHang.DataAccess.Entities
{
    public class LoaiSanPham
    {
        [Key]
        public int MaLoai { get; set; }

        [Required]
        [StringLength(100)]
        public string Loai { get; set; }

        // Navigation property
        public virtual ICollection<SanPham> SanPhams { get; set; }


    }
}
