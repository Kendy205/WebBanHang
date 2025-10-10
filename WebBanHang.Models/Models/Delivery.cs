using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebBanHang.Models.Models
{
    [Table("Delivery")]
    public class Delivery
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int DeliveryId { get; set; }
        [Required]
        [ForeignKey("Order")]
        public int OrderId { get; set; }
        [StringLength(100)]
        public string ShipperName { get; set; }
        [StringLength(20)]
        public string ShipperPhone { get; set; }

        public DateTime? EstimatedTime { get; set; }

        public DateTime? ActualDeliveryTime { get; set; }
        [Required]
        [StringLength(50)]
        public string Status { get; set; } = "Pending";

        [StringLength(500)]
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        public virtual Order Order { get; set; }
    }
}
