using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Project.SmsManager
{
    public class SmsSingleSendDTO
    {
        [Required(ErrorMessage = "انتخاب سرشماره الزامی است.")]
        public string SenderNumberId { get; set; }

        [Required(ErrorMessage = "شماره گیرنده الزامی است.")]
        [StringLength(20, ErrorMessage = "شماره گیرنده نامعتبر است.")]
        public string ReceiverNumber { get; set; }

        [Required(ErrorMessage = "متن پیام الزامی است.")]
        [StringLength(1000, ErrorMessage = "حداکثر 1000 کاراکتر مجاز است.")]
        public string Text { get; set; }
    }
}
