using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Project.SmsConsumer
{
    public class SmsSendBulkDTO
    {
        [Required(ErrorMessage = "سرشماره الزامی است.")]
        public string SenderNumberId { get; set; }

        [Required(ErrorMessage = "متن پیامک الزامی است.")]
        public string Text { get; set; }

        // کاربر در UI متن می‌دهد، ما به array تبدیل می‌کنیم
        [Required(ErrorMessage = "لیست شماره‌ها الزامی است.")]
        public string ReceiverNumbersText { get; set; }
    }
}
