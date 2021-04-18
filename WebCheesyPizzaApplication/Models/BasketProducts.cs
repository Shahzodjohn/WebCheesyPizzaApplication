using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebCheesyPizzaApplication.Models
{
    public class BasketProducts
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int Amount { get; set; }
        public int BasketId { get; set; }
        public virtual Basket Basket { get; set; }
        public virtual Product product { get; set; }
    }
}
