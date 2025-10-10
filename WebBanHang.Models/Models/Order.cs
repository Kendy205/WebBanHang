using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebBanHang.DAL.Data;

namespace WebBanHang.Models.Models
{
    [Table("Order")]
    public class Order
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int OrderId { get; set; }

        [Required]
        [StringLength(20)]
        
        public string OrderCode { get; set; }

        [Required]
        [ForeignKey("User")]
     
        public string UserId { get; set; } // Đổi thành string cho Identity

        public DateTime OrderDate { get; set; } = DateTime.Now;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0, double.MaxValue)]
        public decimal TotalAmount { get; set; }

        [Required]
        [StringLength(50)]
        
        public string Status { get; set; } = "Pending";

        [StringLength(50)]
        public string PaymentMethod { get; set; }

        [Required]
        [StringLength(255)]
        public string ShippingAddress { get; set; }

        [Required]
        [StringLength(20)]
        public string PhoneNumber { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        // Navigation Properties
        public virtual ApplicationUser User { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        public virtual Payment Payment { get; set; }
        public virtual Delivery Delivery { get; set; }
    }
}
