using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Project.AdminUser
{
    /// <summary>
    /// ViewModel فرم ایجاد/ویرایش کاربر مدیر
    /// </summary>
    public class AdminUserFormViewModel
    {
        public AdminUserCreateDTO CreateModel { get; set; }
        public AdminUserEditDTO EditModel { get; set; }

        public List<SelectListItem> UnitOptions { get; set; }
        public List<SelectListItem> RoleOptions { get; set; }
        public List<AdminSecurityQuestionOptionDTO> SecurityQuestionOptions { get; set; }
    }

    public class AdminSecurityQuestionOptionDTO
    {
        public string Id { get; set; }
        public string QuestionText { get; set; }
    }
}
