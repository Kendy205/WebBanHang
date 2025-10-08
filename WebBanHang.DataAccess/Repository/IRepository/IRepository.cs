using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebBanHang.DataAccess.Repository.IRepository
{
    public interface IRepository<T> where T : class
    {
        Task<T> Find(string id);
        Task<IEnumerable<T>> GetAllAsync();
       
        Task AddAsync(T entity);
        Task DeleteAsync(string id);
    }
}
