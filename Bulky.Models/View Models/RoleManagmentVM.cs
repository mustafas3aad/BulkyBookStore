using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.Models.View_Models
{
    public class RoleManagmentVM
    {
        public ApplicationUser ApplicationUser { get; set; }
        
        public IEnumerable<SelectListItem> RoleList { get; set; }
        
        public IEnumerable<SelectListItem> CompanyList { get; set; }
       


    }
}
