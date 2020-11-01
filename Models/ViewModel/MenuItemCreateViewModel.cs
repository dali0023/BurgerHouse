using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace spice.Models.ViewModel
{
    public class MenuItemCreateViewModel
    {
        // Firstly Declare for MenuItem For form for writing name and Id by asp-for
        public MenuItem MenuItem { get; set; }

        // for getting data from Category table
        public SelectList CategoryLists { get; set; }

        // for getting data from SubCategory table
        //public SelectList SubCategoryLists { get; set; }
        public IEnumerable<SubCategory> SubCategoryLists { get; set; }
    }
}
