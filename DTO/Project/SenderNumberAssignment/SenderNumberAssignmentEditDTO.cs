using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Project.SenderNumberAssignment
{
    public class SenderNumberAssignmentEditDTO
    {
        [Required]
        public string Id { get; set; }

        [Required(ErrorMessage = "توضیحات را وارد کنید.")]
        [StringLength(500, ErrorMessage = "توضیحات نمی‌تواند بیش از 500 کاراکتر باشد.")]
        public string Description { get; set; }
    }
}
