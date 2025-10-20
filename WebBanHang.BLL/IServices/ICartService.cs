using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebBanHang.Models.Models;

namespace WebBanHang.BLL.IServices
{
    public interface ICartService
    {
        Task<Cart> GetCartByUserId(string userId);
        Task AddToCart(string userId, int foodId, int quantity);
        Task UpdateCartItem(int cartItemId, int quantity);
        Task RemoveFromCart(int cartItemId);
        Task ClearCart(string userId);
        Task<int> GetCartItemCount(string userId);
        Task<decimal> GetCartTotal(string userId);
        Task<Cart> GetCartByUserIdAsync(string userId);
    }
}