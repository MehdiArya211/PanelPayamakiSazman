using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Project.Menus
{
    public class MenuCreateDTO
    {
        [Required(ErrorMessage = "کلید (Key) منو الزامی است.")]
        [StringLength(100, ErrorMessage = "کلید منو نمی‌تواند بیش از 100 کاراکتر باشد.")]
        public string Key { get; set; }

        [Required(ErrorMessage = "عنوان منو الزامی است.")]
        [StringLength(200, ErrorMessage = "عنوان منو نمی‌تواند بیش از 200 کاراکتر باشد.")]
        public string Title { get; set; }

        [StringLength(100, ErrorMessage = "آیکون نمی‌تواند بیش از 100 کاراکتر باشد.")]
        public string? Icon { get; set; }

        [StringLength(200, ErrorMessage = "کلید مسیر نمی‌تواند بیش از 200 کاراکتر باشد.")]
        public string? RouteKey { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "ترتیب باید عددی مثبت باشد.")]
        public int Order { get; set; } = 1;

        public string? ParentId { get; set; } // اگر null باشد، منوی ریشه است

        public bool IsActive { get; set; } = true;
    }
}
