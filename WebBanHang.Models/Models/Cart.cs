using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebBanHang.DAL.Data;

namespace WebBanHang.Models.Models
{
    public class Cart
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CartId { get; set; }
        [Required]
        [ForeignKey("User")]
        
        public string UserId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        public virtual ApplicationUser User { get; set; }
        public virtual ICollection<CartItem> CartItems { get; set; }
        [NotMapped]
        public decimal TotalAmount
        {
            get
            {
                decimal total = 0;
                if (CartItems != null)
                {
                    foreach (var item in CartItems)
                    {
                        total += item.Subtotal;
                    }
                }
                return total;
            }
        }

        [NotMapped]
        public int TotalItems
        {
            get
            {
                return CartItems?.Count ?? 0;
            }
        }
    }
}
