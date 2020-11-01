using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace spice.Models.ViewModel
{
    public class OrderDetailsViewModel
    {
        public OrderHeader OrderHeader { get; set; }
        public List<OrderDetails> OrderDetails { get; set; }
    }
}
