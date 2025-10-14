using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebBanHang.DAL.Data;
using WebBanHang.DAL.Repository.IRepository;
using WebBanHang.Models.Models;

namespace WebBanHang.DAL.Repository
{
    public class CartRepository : Repository<Cart>, ICartRepository
    {
        private readonly ApplicationDbContext _db;
        public CartRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task<Cart> GetCartByUserIdAsync(string userId)
        {
            if(string.IsNullOrEmpty(userId))
            {
                throw new ArgumentException("User ID cannot be null or empty", nameof(userId));
            }
            return  await _db.Carts.FirstOrDefaultAsync(c => c.UserId == userId);
        }
    }
}
