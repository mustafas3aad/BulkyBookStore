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
    public class OrderDetailReposaitory : Reposaitory<OrderDetail>, IOrderDetailReposaitory
    {
        private readonly ApplicationDbContext db;

        public OrderDetailReposaitory(ApplicationDbContext db):base(db)
        {
            this.db = db;
        }
        public void update(OrderDetail obj)
        {
            db.OrderDetails.Update(obj);
        }
    }
}
