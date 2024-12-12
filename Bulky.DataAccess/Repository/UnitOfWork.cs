using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IReposaitory;
using Bulky.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.Repository
{

    public class UnitOfWork : IUnitofWork
    {
        private readonly ApplicationDbContext db;
       
        public ICategoryReposaitory Category {  get; private set; }

        public IProductReposaitory product {  get; private set; }
        

        public ICompanyReposaitory Company {  get; private set; }
        public IApplicationUserReposaitory ApplicationUser { get; private set; }

        public IShoppingCartReposaitory ShoppingCart { get; private set; }
        public IOrderHeaderReposaitory OrderHeader { get; private set; }
        public IOrderDetailReposaitory OrderDetail { get; private set; }
        public IProductImageReposaitory ProductImage { get; private set; }

        public UnitOfWork(ApplicationDbContext db)
        {
            this.db = db;
            Category=new CategoryReposaitory(db);
            product = new ProductReposaitory(db);
            Company = new CompanyReposaitory(db);
            ShoppingCart = new ShoppingCartReposaitory(db);
            OrderHeader = new OrderHeaderReposaitory(db);
            OrderDetail = new OrderDetailReposaitory(db);
            ApplicationUser = new ApplicationUserReposaitory(db);
            ProductImage = new ProductImageReposaitory(db);
        }
        public void Save()
        {
            db.SaveChanges();
        }
    }
}
