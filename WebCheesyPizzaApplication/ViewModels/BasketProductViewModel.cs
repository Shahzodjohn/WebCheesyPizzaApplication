using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebCheesyPizzaApplication.ViewModels
{
    public class BasketProductViewModel
    {
        public int Id { get; set; }
        public string  Name { get; set; }
        public string ImagePath { get; set; }
        public double Price { get; set; }
        public int Amount { get; set; }
    }
}
