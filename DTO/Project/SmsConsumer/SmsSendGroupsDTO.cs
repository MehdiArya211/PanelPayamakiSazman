using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Project.SmsConsumer
{
    public class SmsSendGroupsDTO
    {
        [Required(ErrorMessage = "سرشماره الزامی است.")]
        public string SenderNumberId { get; set; }

        [Required(ErrorMessage = "متن پیامک الزامی است.")]
        public string Text { get; set; }

        [Required(ErrorMessage = "انتخاب حداقل یک گروه الزامی است.")]
        public List<string> ContactGroupIds { get; set; } = new();
    }
}
