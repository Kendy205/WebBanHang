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

        public async Task CancelOrder(int orderId)
        {
            await UpdateOrderStatus(orderId, "Cancelled");
        }

        public async Task<int> CreateOrderFromCart(string userId, string shippingAddress, string phoneNumber, string paymentMethod, string notes)
        {
            var cart = await _cartService.GetCartByUserId(userId);
            if (cart?.CartItems == null || cart.CartItems.Count == 0)
                throw new Exception("Giỏ hàng trống");

            var order = new Order
            {
                // KHÔNG tự gán OrderId nếu dùng Identity!
                UserId = userId,
                OrderDate = DateTime.UtcNow,
                ShippingAddress = shippingAddress,
                PhoneNumber = phoneNumber,
                PaymentMethod = paymentMethod,
                Notes = notes,
                OrderCode = $"ORD{DateTime.UtcNow:yyMMddHHmmssfff}", // <= 20 ký tự
                Status = "Pending",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                TotalAmount = cart.TotalAmount
            };

            await _unitOfWork.Orders.AddAsync(order);
            try { await _unitOfWork.SaveAsync(); }
            catch (DbUpdateException ex)
            {
                throw new Exception("Save Order failed: " + (ex.InnerException?.Message ?? ex.Message), ex);
            }

            foreach (var i in cart.CartItems)
            {
                // Lấy tên món an toàn
                string foodName = i.Food?.FoodName;
                if (string.IsNullOrWhiteSpace(foodName))
                {
                    var food = await _unitOfWork.Foods.GetByIdAsync(i.FoodId);
                    foodName = food?.FoodName ?? "Sản phẩm";
                }

                var od = new OrderDetail
                {
                    // KHÔNG gán OrderDetailId nếu dùng Identity
                    OrderId = order.OrderId,
                    FoodId = i.FoodId,
                    FoodName = foodName,
                    Quantity = i.Quantity,
                    Price = i.Price
                };
                await _unitOfWork.OrderDetails.AddAsync(od);
            }
            try { await _unitOfWork.SaveAsync(); }
            catch (DbUpdateException ex)
            {
                throw new Exception("Save OrderDetails failed: " + (ex.InnerException?.Message ?? ex.Message), ex);
            }

            var payment = new Payment
            {
                OrderId = order.OrderId,
                PaymentMethod = paymentMethod,
                Amount = cart.TotalAmount,
                Status = "Pending",
                TransactionId = "0",
                CreatedAt = DateTime.UtcNow
            };
            await _unitOfWork.Payments.AddAsync(payment);
            try { await _unitOfWork.SaveAsync(); }
            catch (DbUpdateException ex)
            {
                throw new Exception("Save Payment failed: " + (ex.InnerException?.Message ?? ex.Message), ex);
            }

            await _cartService.ClearCart(userId);
            return order.OrderId;

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

        public async Task UpdateOrderStatus(int orderId, string status)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(orderId);
            if (order != null)
            {
                order.Status = status;
                order.UpdatedAt = DateTime.Now;
                _unitOfWork.Orders.Update(order);

                // Update payment status if order is completed
                if (status == "Completed")
                {
                    var payment =await _unitOfWork.Payments.FirstOrDefaultAsync(p => p.OrderId == orderId);
                    if (payment != null)
                    {
                        payment.Status = "Completed";
                        payment.PaymentDate = DateTime.Now;
                        _unitOfWork.Payments.Update(payment);
                    }
                }
                await _unitOfWork.SaveAsync();
                
            }
        }

        public Task<bool> UserOwnsOrder(string userId, int orderId)
        {
            return _unitOfWork.Orders.GetAllQueryable().AnyAsync(o => o.OrderId == orderId && o.UserId == userId);
        }
        // ==============================
        // Lấy danh sách đơn hàng theo UserId
        // ==============================
        public async Task<IEnumerable<Order>> GetOrdersByUserIdAsync(string userId)
        {
            // Dùng IQueryable để lọc/sort rồi ToListAsync
            return await _unitOfWork.Orders
                .GetAllQueryable()
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        public async Task<Order?> GetOrderByIdAsync(int id)
        {
            // Lấy kèm items + food nếu cần hiển thị chi tiết
            return await _unitOfWork.Orders
                .GetAllQueryable()
                .Include(o => o.OrderDetails)
                    .ThenInclude(i => i.Food)     
                .FirstOrDefaultAsync(o => o.OrderId == id);
        }
    }
}
