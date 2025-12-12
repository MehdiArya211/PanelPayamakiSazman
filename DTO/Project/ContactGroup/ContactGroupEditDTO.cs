using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Project.ContactGroup
{
    public class ContactGroupEditDTO
    {
        public string Id { get; set; } // از API/لیست استخراج می‌شود

        [Required(ErrorMessage = "نام گروه الزامی است.")]
        [StringLength(100, ErrorMessage = "نام گروه نمی‌تواند بیشتر از 100 کاراکتر باشد.")]
        public string Name { get; set; }

        [StringLength(500, ErrorMessage = "توضیحات نمی‌تواند بیشتر از 500 کاراکتر باشد.")]
        public string? Description { get; set; }
    }
}
