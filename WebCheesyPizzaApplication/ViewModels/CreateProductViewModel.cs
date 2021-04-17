using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebCheesyPizzaApplication.ViewModels
{
    public class CreateProductViewModel
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public virtual IFormFile Image { get; set; }
        [Required(ErrorMessage ="Выберите фото")]
        public int CategoryId { get; set; }
        public double Price { get; set; }
        public string Description { get; set; }
        public string Reciept { get; set; }

        public Dictionary<int, string> Categories { get; set; }




    }
}
