using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebBanHang.BLL.IServices;
using WebBanHang.DAL.Repository.UnitOfWork;
using WebBanHang.Models.Models;

namespace WebBanHang.BLL.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICartService _cartService;
        public OrderService(IUnitOfWork unitOfWork, ICartService cartService)
        {
            _unitOfWork = unitOfWork;
            _cartService = cartService;
        }

        public Task CancelOrder(int orderId)
        {
            throw new NotImplementedException();
        }

        public Task<int> CreateOrderFromCart(string userId, string shippingAddress, string phoneNumber, string paymentMethod, string notes)
        {
            throw new NotImplementedException();
        }

        public  async Task<IEnumerable<Order>> GetAllOrders()
        {
            return await  _unitOfWork.Orders.GetAllQueryable()
                .Include(o => o.User)
                .Include( o=> o.OrderDetails)
                .ToListAsync();

        }

        public async Task<Order> GetOrderByCode(string orderCode)
        {
            return await _unitOfWork.Orders.FirstOrDefaultAsync(o => o.OrderCode == orderCode);
        }

        public async Task<Order> GetOrderById(int id)
        {
            return await _unitOfWork.Orders.GetAllQueryable()
                .Include(o => o.User)
                .Include(o => o.OrderDetails).ThenInclude(od => od.Food)
                .Include(o => o.Payment)
                .FirstOrDefaultAsync(o => o.OrderId == id);
                
        }

        public async Task<IEnumerable<Order>> GetOrdersByStatus(string status)
        {
            return await _unitOfWork.Orders.FindAsync(o => o.Status== status);
        }

        public async Task<IEnumerable<Order>> GetOrdersByUserId(string userId)
        {
            return await _unitOfWork.Orders.GetAllQueryable()
                .Include(o => o.OrderDetails).ThenInclude(od => od.Food)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        public Task UpdateOrderStatus(int orderId, string status)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UserOwnsOrder(string userId, int orderId)
        {
            throw new NotImplementedException();
        }
    }
}
