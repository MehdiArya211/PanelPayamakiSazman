using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Project.SenderNumberAssignment
{
    public class SenderNumberAssignmentCreateDTO
    {
        [Required(ErrorMessage = "انتخاب سرشماره الزامی است.")]
        public string SenderNumberId { get; set; }

        [Required(ErrorMessage = "انتخاب کاربر الزامی است.")]
        public string UserId { get; set; }

        public string ClientId { get; set; }

        [Required(ErrorMessage = "توضیحات را وارد کنید.")]
        [StringLength(500, ErrorMessage = "توضیحات نمی‌تواند بیش از 500 کاراکتر باشد.")]
        public string Description { get; set; }
    }
}
