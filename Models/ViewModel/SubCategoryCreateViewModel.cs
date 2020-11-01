using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace spice.Models.ModelView
{
    public class SubCategoryCreateViewModel
    {
        public SelectList CategoryLists { get; set; }
        public List<string> SubCategoryList { get; set; }
        public SubCategory SubCategory { get; set; }
        public string StatusMessage { get; set; }

    }
}
