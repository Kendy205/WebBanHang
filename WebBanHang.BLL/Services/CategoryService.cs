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
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        public CategoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task AddCategory(Category category)
        {
            await _unitOfWork.Categories.AddAsync(category);
            await _unitOfWork.SaveAsync();
        }

        public async Task<bool> CategoryExists(int id)
        {
            return await _unitOfWork.Categories.CategoryExists(id);
        }

        public async Task DeleteCategory(int id)
        {
            await _unitOfWork.Categories.DeleteAsync(id);
            await _unitOfWork.SaveAsync();
        }

        public Task<IEnumerable<Category>> GetActiveCategories()
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Category>> GetAllCategories()
        {
            return await _unitOfWork.Categories.GetAllAsync();
        }

        public async Task<Category> GetCategoryById(int id)
        {
            return await _unitOfWork.Categories.GetByIdAsync(id);
        }

        public async Task UpdateCategory(Category category)
        {
            _unitOfWork.Categories.Update(category);
            await _unitOfWork.SaveAsync();
            //return Task.CompletedTask;
        }

       
    }
}
