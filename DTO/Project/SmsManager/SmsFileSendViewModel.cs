using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Project.SmsManager
{
    public class SmsFileSendViewModel
    {
        [Required(ErrorMessage = "انتخاب سرشماره الزامی است.")]
        public string SenderNumberId { get; set; }

        [Required(ErrorMessage = "فایل الزامی است.")]
        public IFormFile File { get; set; }

        [Required(ErrorMessage = "متن پیام الزامی است.")]
        [StringLength(1000, ErrorMessage = "حداکثر 1000 کاراکتر مجاز است.")]
        public string Text { get; set; }
    }
}
