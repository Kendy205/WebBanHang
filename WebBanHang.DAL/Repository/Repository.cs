using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using WebBanHang.DAL.Data;
using WebBanHang.DAL.Repository.IRepository;

namespace WebBanHang.DAL.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _db;
        internal DbSet<T> DbSet;
        public Repository(ApplicationDbContext db)
        {
            _db = db;
            this.DbSet = _db.Set<T>();
        }
        public async Task AddAsync(T entity)
        {
            await DbSet.AddAsync(entity);
        }

        public async Task<int> Count()
        {
            return await DbSet.CountAsync();    
        }

        public async Task<int> Count(Expression<Func<T, bool>> predicate)
        {
            return await DbSet.CountAsync(predicate);
        }

        public async Task DeleteAsync(object id)
        {
            DbSet.Remove(await FindAsync(id));
        }

        public async Task<T> FindAsync(object id)
        {
            return await DbSet.FindAsync(id);
        }

        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> filter)
        {
            return await DbSet.Where(filter).ToListAsync();
        }

        public async Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> filter)
        {
            return await DbSet.Where(filter).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            IEnumerable<T> query = await DbSet.ToListAsync();
            return query;
        }

        public IQueryable<T> GetAllQueryable()
        {
            return DbSet.AsQueryable();

        }

        public async Task<T> GetByIdAsync(object id)
        {
            return await DbSet.FindAsync(id);
        }

        public void Update(T entity)
        {
             DbSet.Update(entity);
        }

       
    }
}
