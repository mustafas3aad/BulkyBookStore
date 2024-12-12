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
    public class OrderHeaderReposaitory : Reposaitory<OrderHeader>, IOrderHeaderReposaitory
    {
        private readonly ApplicationDbContext db;

        public OrderHeaderReposaitory(ApplicationDbContext db):base(db)
        {
            this.db = db;
        }
        public void update(OrderHeader obj)
        {
            db.OrderHeaders.Update(obj);
        }

     
		public void UpdateStatus(int id, string orderStatus, string? paymentStatus = null)
		{
			var orderFromDb = db.OrderHeaders.FirstOrDefault(x => x.Id == id);
            if (orderFromDb != null)
            {
                orderFromDb.OrderStatus = orderStatus;
                if(!string.IsNullOrEmpty(paymentStatus))
                {
                    orderFromDb.PaymentStatus = paymentStatus;
                }
            }
		}


		public void UpdateStripPaymentID(int id, string sessionId, string paymentIntentId)
		{
			var orderFromDb = db.OrderHeaders.FirstOrDefault(x => x.Id == id);
            if (!string.IsNullOrEmpty(sessionId))
            {
                orderFromDb.SessionId= sessionId;
            }
            if (!string.IsNullOrEmpty(paymentIntentId))
            {
                orderFromDb.PaymentIntentId= paymentIntentId;
                orderFromDb.PaymentData = DateTime.Now;
            }

        }
	}
}
