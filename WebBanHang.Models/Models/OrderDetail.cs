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
    [Table("OrderDetail")]
    public class OrderDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrderDetailId { get; set; }

        [Required]
        [ForeignKey("Order")]
       
        public int OrderId { get; set; }

        [Required]
        [ForeignKey("Food")]
        public int FoodId { get; set; }

        [Required]
        [StringLength(150)]
        public string FoodName { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [NotMapped]
        public decimal Subtotal => Quantity * Price;

        // Navigation Properties
        public virtual Order Order { get; set; }
        public virtual Food Food { get; set; }
    }
}
