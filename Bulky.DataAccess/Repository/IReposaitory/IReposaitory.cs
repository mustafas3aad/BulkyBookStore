using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;



namespace Bulky.DataAccess.Repository.IReposaitory
{
    public interface IReposaitory<T> where T : class
    {
     

        IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter=null, string? includeProperties = null);
        
        T Get(Expression<Func<T,bool>> filter, string? includeProperties = null, bool tracked = false);
        void Add(T entity);
        
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entity);
    }
}
