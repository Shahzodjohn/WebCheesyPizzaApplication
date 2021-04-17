using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebCheesyPizzaApplication.ViewModels
{
    public class IndexCategoryViewModel
    {
        public List<CategoryViewModel> Categories { get; set; }
        public List<ProductViewModel> Products { get; set; }
    }
}
