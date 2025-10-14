using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebBanHang.Models.Models;

namespace WebBanHang.BLL.IServices
{
    public interface ICategoryService
    {
        Task<IEnumerable<Category>> GetAllCategories();
        Task<IEnumerable<Category>> GetActiveCategories();
        Task<Category> GetCategoryById(int id);
        Task AddCategory(Category category);
        Task UpdateCategory(Category category);
        Task DeleteCategory(int id);
        Task<bool> CategoryExists(int id);
    }
}
