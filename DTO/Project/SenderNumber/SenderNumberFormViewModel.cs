using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Project.SenderNumber
{
    /// <summary>
    /// ViewModel فرم ایجاد/ویرایش سرشماره (DTO + لیست‌ها)
    /// </summary>
    public class SenderNumberFormViewModel
    {
        public SenderNumberCreateDTO CreateModel { get; set; }  // در Create
        public SenderNumberEditDTO EditModel { get; set; }      // در Edit

        public List<SelectListItem> SpecialtyOptions { get; set; }
        public List<SelectListItem> OrganizationLevelOptions { get; set; }
        public List<SelectListItem> SubAreaOptions { get; set; }
        public List<SelectListItem> StatusOptions { get; set; }
    }
}
