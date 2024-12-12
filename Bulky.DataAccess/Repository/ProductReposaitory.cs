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
    public class ProductReposaitory : Reposaitory<Product>, IProductReposaitory
    {
        private readonly ApplicationDbContext db;

        public ProductReposaitory(ApplicationDbContext db):base(db)
        {
            this.db = db;
        }
        public void update(Product obj)
        {
            var objFromDb = db.products.FirstOrDefault(u => u.Id == obj.Id);
            if (objFromDb != null)
            {
                objFromDb.Title = obj.Title;
                objFromDb.Description = obj.Description;
                objFromDb.CategoryId = obj.CategoryId;
                objFromDb.ISBN = obj.ISBN;
                objFromDb.Price = obj.Price;
                objFromDb.ListPrice = obj.ListPrice;
                objFromDb.Price50 = obj.Price50;
                objFromDb.Price100 = obj.Price100;
                objFromDb.Author= obj.Author;
               
                objFromDb.ProductImages= obj.ProductImages;
            }
            
        }
    }
}
