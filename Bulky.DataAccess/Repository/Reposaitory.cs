using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IReposaitory;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;


namespace Bulky.DataAccess.Repository
{
    
    public class Reposaitory<T> : IReposaitory<T> where T : class
    {
        private readonly ApplicationDbContext db;
        internal DbSet<T> dbSet;

        public Reposaitory(ApplicationDbContext db)
        {
            this.db = db;
            
            this.dbSet = db.Set<T>();
          
            db.products.Include(u => u.Category).Include(u => u.CategoryId);

        }
        public void Add(T entity)
        {
            db.Add(entity);
        }

        public T Get(Expression<Func<T, bool>> filter, string? includeProperties= null, bool tracked = false)
        {
            IQueryable<T> query;
            
            if (tracked)
            {
                 query = dbSet;
            }
            else
            {
                 query = dbSet.AsNoTracking();
            }
            query = query.Where(filter);
            
            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var includeProp in includeProperties
                    .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp);
                }
            }
            return query.FirstOrDefault();
        }

        public IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter, string? includeProperties= null)
        {
            IQueryable<T> query = dbSet;
            if(filter != null)
            {
                query = query.Where(filter);
            }
    
            
            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var includeProp in includeProperties
                    .Split(new char[] { ',' } , StringSplitOptions.RemoveEmptyEntries))
                {
                    query =query.Include(includeProp);
                }
            }
            return query.ToList();
        }

        public void Remove(T entity)
        {
            dbSet.Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entity)
        {
            dbSet.RemoveRange(entity);
        }
    }
}
