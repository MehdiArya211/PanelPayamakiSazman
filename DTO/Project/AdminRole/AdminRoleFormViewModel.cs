using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Project.AdminRole
{
    /// <summary>
    /// ViewModel فرم ایجاد/ویرایش نقش
    /// </summary>
    public class AdminRoleFormViewModel
    {
        public AdminRoleCreateDTO CreateModel { get; set; }
        public AdminRoleEditDTO EditModel { get; set; }

        public List<SelectListItem> StatusOptions { get; set; }

        // می‌تواند لیست permission ها هم اضافه شود
        public List<SelectListItem> PermissionOptions { get; set; }
    }
}
