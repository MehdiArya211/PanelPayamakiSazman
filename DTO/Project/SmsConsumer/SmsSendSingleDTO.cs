using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Project.SmsConsumer
{
    public class SmsSendSingleDTO
    {
        [Required(ErrorMessage = "سرشماره الزامی است.")]
        public string SenderNumberId { get; set; }

        [Required(ErrorMessage = "شماره گیرنده الزامی است.")]
        public string ReceiverNumber { get; set; }

        [Required(ErrorMessage = "متن پیامک الزامی است.")]
        public string Text { get; set; }
    }
}
