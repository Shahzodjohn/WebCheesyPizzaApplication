using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebCheesyPizzaApplication.ViewModels
{
    public class OrderListViewModel
    {
        public List<OrderViewModel> Orders { get; set; }
        public Dictionary<int,string> OrderStates { get; set; }
    }
}
