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
    public class ShoppingCartReposaitory : Reposaitory<ShoppingCart>, IShoppingCartReposaitory
    {
        private readonly ApplicationDbContext db;

        public ShoppingCartReposaitory(ApplicationDbContext db):base(db)
        {
            this.db = db;
        }
        public void update(ShoppingCart obj)
        {
            db.ShoppingCarts.Update(obj);
        }
    }
}
