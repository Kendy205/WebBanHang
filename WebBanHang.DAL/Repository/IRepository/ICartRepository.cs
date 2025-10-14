using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebBanHang.Models.Models;

namespace WebBanHang.DAL.Repository.IRepository
{
    public interface ICartRepository : IRepository<Cart>
    {
        Task<Cart> GetCartByUserIdAsync(string userId);
    }
}
