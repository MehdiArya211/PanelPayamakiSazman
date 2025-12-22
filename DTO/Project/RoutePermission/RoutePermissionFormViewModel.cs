using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Project.RoutePermission
{
    /// <summary>
    /// ViewModel فرم ویرایش مجوز
    /// </summary>
    public class RoutePermissionFormViewModel
    {
        public RoutePermissionEditDTO EditModel { get; set; }

        public List<SelectListItem> AvailableActions { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> FreshAuthOptions { get; set; } = new List<SelectListItem>();
    }
}
