using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebBanHang.Models.Models;

namespace WebBanHang.BLL.IServices
{
    public interface IOrderService
    {
        Task<IEnumerable<Order>> GetAllOrders();
        Task<IEnumerable<Order>> GetOrdersByUserId(string userId);
        Task<IEnumerable<Order>> GetOrdersByStatus(string status);
        Task<Order> GetOrderById(int id);
        Task<Order> GetOrderByCode(string orderCode);
        Task<int> CreateOrderFromCart(string userId, string shippingAddress, string phoneNumber,
            string paymentMethod, string notes);
        Task UpdateOrderStatus(int orderId, string status);
        Task CancelOrder(int orderId);
        Task<bool> UserOwnsOrder(string userId, int orderId);
        Task<IEnumerable<Order>> GetOrdersByUserIdAsync(string userId);
        Task<Order?> GetOrderByIdAsync(int id);
    }
}
