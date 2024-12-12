using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.Repository.IReposaitory
{
    public interface IUnitofWork
    {
        public ICategoryReposaitory Category { get; }
        public IProductReposaitory product { get; }
        public ICompanyReposaitory Company { get;}
        public IShoppingCartReposaitory ShoppingCart { get; }
        public IOrderDetailReposaitory OrderDetail { get; }
        public IOrderHeaderReposaitory OrderHeader { get; }
        public IApplicationUserReposaitory ApplicationUser { get; }
        public IProductImageReposaitory ProductImage { get; }
        void Save();
    }
}
