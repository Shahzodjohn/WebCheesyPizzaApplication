using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebCheesyPizzaApplication.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public DateTime OrderDate { get; set; }
        public int OrderStateId { get; set; }
        public virtual OrderState OrderState { get; set; }
        public virtual ICollection<OrderProduct> OrderProducts{get;set;}
    }
}
