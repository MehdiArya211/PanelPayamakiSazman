using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Project.SenderNumber
{
    public class SenderNumberCreateDTO
    {
        [Required(ErrorMessage = "وارد کردن پیش‌شماره الزامی است.")]
        [StringLength(20, ErrorMessage = "پیش‌شماره نمی‌تواند بیش از 20 کاراکتر باشد.")]
        public string FixedPrefix { get; set; }

        [Required(ErrorMessage = "انتخاب حوزه تخصصی الزامی است.")]
        public string SpecialtyId { get; set; }

        [Required(ErrorMessage = "انتخاب رده سازمانی الزامی است.")]
        public string OrganizationLevelId { get; set; }

        [Required(ErrorMessage = "انتخاب حوزه فرعی الزامی است.")]
        public string SubAreaId { get; set; }

        [Required(ErrorMessage = "انتخاب وضعیت الزامی است.")]
        public string Status { get; set; }  // Purchasing, CompletingDocuments, ReviewingDocuments, Active, Inactive

        [StringLength(500, ErrorMessage = "توضیحات نمی‌تواند بیش از 500 کاراکتر باشد.")]
        public string? Description { get; set; }
    }
}
