using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebBanHang.Models.Models;

namespace WebBanHang.BLL.IServices
{
    public interface IFoodService
    {
        Task<IEnumerable<Food>> GetAllFoods();
        Task<IEnumerable<Food>> GetFoodsByCategory(int categoryId);
        Task<IEnumerable<Food>> GetAvailableFoods();
        Task<IEnumerable<Food>> SearchFoods(string keyword);
        Task<IEnumerable<Food>> GetTopRatedFoods(int count);
        Task<(IEnumerable<Food> foods, int totalRecords)> GetFoodsPaged(int pageNumber, int pageSize);
        Task<(IEnumerable<Food> foods, int totalRecords)> GetFoodsByFilter(int? categoryId, decimal? minPrice, decimal? maxPrice,
            string sortBy, int pageNumber, int pageSize);
        Task<Food> GetFoodById(int id);
        Task AddFood(Food food);
        Task UpdateFood(Food food);
        Task DeleteFood(int id);
        Task<bool> FoodExists(int id);
        Task UpdateRating(int foodId);
    }
}
