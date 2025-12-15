using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Project.Menus
{
    public class MenuFormViewModel
    {
        public MenuCreateDTO CreateModel { get; set; }
        public MenuEditDTO EditModel { get; set; }
        public List<SelectListItem> ParentOptions { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> ActiveOptions { get; set; } = new List<SelectListItem>();
    }
}
