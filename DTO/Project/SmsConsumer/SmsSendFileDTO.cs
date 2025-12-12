using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Project.SmsConsumer
{
    public class SmsSendFileDTO
    {
        [Required(ErrorMessage = "سرشماره الزامی است.")]
        public string SenderNumberId { get; set; }

        [Required(ErrorMessage = "متن پیامک الزامی است.")]
        public string Text { get; set; }

        [Required(ErrorMessage = "فایل الزامی است.")]
        public IFormFile File { get; set; }
    }
}
