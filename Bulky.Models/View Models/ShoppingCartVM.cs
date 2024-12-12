using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.Models
{
    public class ShoppingCartVM
    {
        public IEnumerable<ShoppingCart> shoppingCartList { get; set; }
        
        public OrderHeader orderHeader { get; set; }

        
    }
}
