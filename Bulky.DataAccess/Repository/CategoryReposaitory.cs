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
    public class CategoryReposaitory : Reposaitory<Category>, ICategoryReposaitory
    {
        private readonly ApplicationDbContext db;

        public CategoryReposaitory(ApplicationDbContext db):base(db)
        {
            this.db = db;
        }
        public void update(Category obj)
        {
            db.Categories.Update(obj);
        }
    }
}
