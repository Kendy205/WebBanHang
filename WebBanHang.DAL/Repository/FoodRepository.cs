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
    public class FoodRepository : Repository<Food>, IFoodRepository
    {
        private readonly ApplicationDbContext _db;
        public FoodRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
    }
}
