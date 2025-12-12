using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Project.SmsConsumer
{
    public class SmsSendViewModel
    {
        public SmsSendSingleDTO Single { get; set; } = new();
        public SmsSendBulkDTO Bulk { get; set; } = new();
        public SmsSendGroupsDTO Groups { get; set; } = new();
        public SmsSendFileDTO File { get; set; } = new();

        public List<SelectListItem> SenderNumberOptions { get; set; } = new();
        public List<SelectListItem> ContactGroupOptions { get; set; } = new();
    }
}
