using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebBanHang.Models.Models
{
    [Table("CartItem")]
    public class CartItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CartItemId { get; set; }

        [Required]
        [ForeignKey("Cart")]
        public int CartId { get; set; }

        [Required]
        [ForeignKey("Food")]
        public int FoodId { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [NotMapped] // Tính toán trong code thay vì computed column
        public decimal Subtotal => Quantity * Price;

        public DateTime AddedAt { get; set; } = DateTime.Now;

        // Navigation Properties
        public virtual Cart Cart { get; set; }
        public virtual Food Food { get; set; }
    }
}
