using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Project.Unit
{
    /// <summary>
    /// مدل فرم ایجاد واحد سازمانی
    /// </summary>
    public class UnitCreateDTO
    {
        [Required(ErrorMessage = "وارد کردن کد الزامی است.")]
        [StringLength(20, ErrorMessage = "کد نمی‌تواند بیش از 20 کاراکتر باشد.")]
        public string Code { get; set; }

        [Required(ErrorMessage = "وارد کردن نام الزامی است.")]
        [StringLength(100, ErrorMessage = "نام نمی‌تواند بیش از 100 کاراکتر باشد.")]
        public string Name { get; set; }

        [StringLength(500, ErrorMessage = "توضیحات نمی‌تواند بیش از 500 کاراکتر باشد.")]
        public string? Description { get; set; }

        [Display(Name = "فعال است")]
        public bool IsActive { get; set; } = true;

        //[Display(Name = "واحد والد")]
        //public string? ParentId { get; set; } // GUID به صورت string، اختیاری
    }
}
