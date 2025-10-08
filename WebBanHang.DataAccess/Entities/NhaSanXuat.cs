using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebBanHang.DataAccess.Entities
{
    public class NhaSanXuat
    {
        [Key]
        [StringLength(20)]
        public string MaNSX { get; set; }
        public string TenNSX { get; set; }
        // Navigation properties
        public virtual ICollection<SanPham> SanPhams { get; set; }
    }
}
