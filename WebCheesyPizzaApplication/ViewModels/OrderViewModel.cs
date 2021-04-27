using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebCheesyPizzaApplication.Models;

namespace WebCheesyPizzaApplication.ViewModels
{
    public class OrderViewModel
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public string UserName { get; set; }
        public double Summ { get; set; }
        public string State { get; set; }
        public string UserId { get; set; }
    }
}
