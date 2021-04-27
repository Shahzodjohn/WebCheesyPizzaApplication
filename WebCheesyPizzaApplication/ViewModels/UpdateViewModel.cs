using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebCheesyPizzaApplication.ViewModels
{
    public class UpdateViewModel : CreateProductViewModel
    {
        public int Id { get; set; }
        public string ImagePath { get; set; }
        public IFormFile? Image { get; set; }
    }
}
