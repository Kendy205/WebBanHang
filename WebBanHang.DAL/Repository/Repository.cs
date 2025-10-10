using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task DeleteAsync(string id)
        {
            DbSet.Remove(await Find(id));
        }

        public async Task<T> Find(string id)
        {
            T entity = await DbSet.FindAsync(id);
            return entity;
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            IQueryable<T> query = DbSet;
            return query;
        }
 
    }
}
