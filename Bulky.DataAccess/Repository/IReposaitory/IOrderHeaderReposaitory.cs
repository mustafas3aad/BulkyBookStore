using Bulky.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.Repository.IReposaitory
{
    public interface IOrderHeaderReposaitory : IReposaitory<OrderHeader>
    {
        void update(OrderHeader obj);

       
        void UpdateStatus(int id, string orderStatus, string? paymentStatus = null);
        void UpdateStripPaymentID(int id, string sessionId, string paymentIntentId);
    }
}
