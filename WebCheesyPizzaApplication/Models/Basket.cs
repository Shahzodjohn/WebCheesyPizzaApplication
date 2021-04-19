using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebCheesyPizzaApplication.Models
{
    public class Basket
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public virtual ICollection<BasketProducts> BasketProducts { get; set; }
    }
}
