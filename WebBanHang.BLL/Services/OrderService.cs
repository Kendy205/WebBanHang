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
            //lay gio hang tu userid qua services
            var cart = await _cartService.GetCartByUserId(userId);
            if (cart.CartItems == null || cart.CartItems.Count == 0)
            {
                throw new Exception("Gio hang trong");
            }
            //tao Order tu Cart
            var orderCode = "ORD" + DateTime.UtcNow.ToString("ddmmyyyy");
            var orderId = await _unitOfWork.Orders.Count() + 1;
            var order = new Order
            {
                OrderId = orderId,
                UserId = userId,
                OrderDate = DateTime.UtcNow,
                ShippingAddress = shippingAddress,
                PhoneNumber = phoneNumber,
                PaymentMethod = paymentMethod,
                Notes = notes,
                OrderCode = orderCode,
                Status = "Pending",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                TotalAmount=cart.TotalAmount,
            };
            await _unitOfWork.Orders.AddAsync(order);
            await _unitOfWork.SaveAsync();

            //create orderdetail
            foreach(var i in cart.CartItems)
            {
                var orderDetail = new OrderDetail
                { 
                    OrderId=order.OrderId,
                    FoodId=i.FoodId,
                    FoodName=i.Food.FoodName,
                    Quantity=i.Quantity,
                    Price=i.Price,
                };
                await _unitOfWork.OrderDetails.AddAsync(orderDetail);
            }
            // tao phuong thuc thanh toan
            var payment = new Payment
            { 
                OrderId=order.OrderId,
                PaymentMethod=paymentMethod,
                Amount=cart.TotalAmount,
                Status="Pending",
                CreatedAt= DateTime.UtcNow,
            };
            await _unitOfWork.Payments.AddAsync(payment);
            await _unitOfWork.SaveAsync();
            //xoa tat ca san pham trong cart
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
