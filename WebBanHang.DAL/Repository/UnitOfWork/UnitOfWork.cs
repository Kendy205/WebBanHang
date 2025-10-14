using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebBanHang.DAL.Data;
using WebBanHang.DAL.Repository.IRepository;
using WebBanHang.Models.Models;

namespace WebBanHang.DAL.Repository.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _db;
        

        public ICategoryRepository Categories { get; private set; }

        public ICartRepository Carts { get; private set; }

        public ICartItemRepository CartItems { get; private set; }

        public IFoodRepository Foods { get; private set; }

        public IOrderRepository Orders { get; private set; }

        public IOrderDetailRepository OrderDetails { get; private set; }

        public IPaymentRepository Payments { get; private set; }
        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            Categories = new CategoryRepository(_db);
            Carts = new CartRepository(_db);
            CartItems = new CartItemRepository(_db);
            Foods = new FoodRepository(_db);
            Orders = new OrderRepository(_db);
            OrderDetails = new OrderDetailRepository(_db);
            Payments = new PaymentRepository(_db);
        }

        public async Task SaveAsync()
        {
            await _db.SaveChangesAsync();
        }

        public void Dispose()
        {
            _db.Dispose();
        }
    }
}
