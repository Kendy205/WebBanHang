using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace WebBanHang.DAL.Repository.IRepository
{
    public interface IRepository<T> where T : class
    {
        
        Task<T> FindAsync(object id);
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> filter);
        Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> filter);
        Task<T> GetByIdAsync(object id);

        Task<IEnumerable<T>> GetAllAsync();
        IQueryable<T> GetAllQueryable();

        Task AddAsync(T entity);
        void Update(T entity);
        Task DeleteAsync(object id);

        Task<int> Count();
        Task<int> Count(Expression<Func<T, bool>> predicate);

    }
}
