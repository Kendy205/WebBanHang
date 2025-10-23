using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebBanHang.BLL.IServices;
using WebBanHang.DAL.Repository.UnitOfWork;
using WebBanHang.Models.Models;

namespace WebBanHang.BLL.Services
{
    public class FoodService : IFoodService
    {
        private readonly IUnitOfWork _unitOfWork; 
        public FoodService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task AddFood(Food food)
        {
            food.CreatedAt = DateTime.Now;
            food.UpdatedAt = DateTime.Now;
            await _unitOfWork.Foods.AddAsync(food);
            await _unitOfWork.SaveAsync();
        }

        public async Task DeleteFood(int id)
        {
            var food = await _unitOfWork.Foods.GetByIdAsync(id);
            if(food != null)
            {
                await _unitOfWork.Foods.DeleteAsync(id);
                await _unitOfWork.SaveAsync();
            }
           
        }

        public async Task<bool> FoodExists(int id)
        {
            return await _unitOfWork.Foods.FindAsync(id) != null;
        }

        public async Task<IEnumerable<Food>> GetAllFoods()
        {
            return await _unitOfWork.Foods.GetAllAsync();
        }

        public async Task<IEnumerable<Food>> GetAvailableFoods()
        {
            return await _unitOfWork.Foods.FindAsync(f => f.IsAvailable);
        }
        

        public async Task<Food> GetFoodById(int id)
        {
            return await _unitOfWork.Foods.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Food>> GetFoodsByCategory(int categoryId)
        {
            //return await _unitOfWork.Foods.FindAsync(f => f.CategoryId == categoryId);
            return await _unitOfWork.Foods.GetAllQueryable().Include(f => f.Category).Where(f => f.CategoryId == categoryId).ToListAsync();
        }

        public async Task<(IEnumerable<Food> foods, int totalRecords)> GetFoodsByFilter(int? categoryId, decimal? minPrice, decimal? maxPrice, string sortBy, int pageNumber, int pageSize)
        {
            // lay all food
            var query = _unitOfWork.Foods.GetAllQueryable();
            // category filter
            if (categoryId.HasValue)
            {
                query = query.Where(f => f.CategoryId == categoryId.Value);
            }
            // price filter
            if (minPrice.HasValue)
            {
                query = query.Where(f => f.Price >= minPrice.Value);
            }
            if (maxPrice.HasValue)
            {
                query = query.Where(f => f.Price <= maxPrice.Value);
            }
            // Sort
            switch (sortBy)
            {
                case "price_asc":
                    query = query.OrderBy(f => f.Price);
                    break;
                case "price_desc":
                    query = query.OrderByDescending(f => f.Price);
                    break;
                case "rating":
                    query = query.OrderByDescending(f => f.Rating);
                    break;
                case "name":
                default:
                    query = query.OrderBy(f => f.FoodName);
                    break;
            }
            var totalRecords = await query.CountAsync();
            var foods = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            return (foods, totalRecords);
        }

        public async Task<(IEnumerable<Food> foods, int totalRecords)> GetFoodsPaged(int pageNumber, int pageSize)
        {
            var query = _unitOfWork.Foods.GetAllQueryable().Include(f => f.Category).Where(f => f.IsAvailable);
            var food = await query.Skip((pageNumber-1)*pageSize).Take(pageSize).ToListAsync();
            var totalRecords = query.Count();
            return (food, totalRecords);
        }

        public async Task<IEnumerable<Food>> GetTopRatedFoods(int count)
        {
            return await _unitOfWork.Foods.GetAllQueryable().OrderByDescending(f => f.Rating).Take(count).ToListAsync();
        }

        public async Task<IEnumerable<Food>> SearchFoods(string keyword)
        {
            if(string.IsNullOrWhiteSpace(keyword))
            {
                return Enumerable.Empty<Food>();
            }
            keyword = keyword.ToLower().Trim();
            return await _unitOfWork.Foods.FindAsync(f=> f.IsAvailable && ( f.FoodName.Contains(keyword) || (f.Description != null && f.Description.Contains(keyword))));

        }

        public async Task UpdateFood(Food food)
        {
             _unitOfWork.Foods.Update(food);
            await _unitOfWork.SaveAsync();
        }

        public async Task UpdateRating(int foodId)
        {
           // var food = await _unitOfWork.Foods.GetByIdAsync(foodId);

        }
    }
}
