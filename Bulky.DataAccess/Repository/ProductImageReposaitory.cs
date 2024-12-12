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
    public class ProductImageReposaitory : Reposaitory<ProductImage>, IProductImageReposaitory
    {
        private readonly ApplicationDbContext db;

        public ProductImageReposaitory(ApplicationDbContext db):base(db)
        {
            this.db = db;
        }
        public void update(ProductImage obj)
        {
            db.ProductImages.Update(obj);
        }
    }
}
