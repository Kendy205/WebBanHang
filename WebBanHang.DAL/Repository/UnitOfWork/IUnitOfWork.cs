using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebBanHang.DAL.Repository.IRepository;

namespace WebBanHang.DAL.Repository.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        ICategoryRepository Categories { get; }
        ICartRepository Carts { get; }
        ICartItemRepository CartItems { get; }
        IFoodRepository Foods { get; }
        IOrderRepository Orders { get; }
        IOrderDetailRepository OrderDetails { get; }
        IPaymentRepository Payments { get; }
        Task SaveAsync();
    }
}
