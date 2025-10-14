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
    public class CartItemRepository :Repository<CartItem>, ICartItemRepository
    {
        private readonly ApplicationDbContext _db;
        public CartItemRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
    }
}
