using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Project.SmsManager
{
    public class AdminSmsIndexViewModel
    {
        public AdminSmsHistoryFilterDTO Filter { get; set; } = new AdminSmsHistoryFilterDTO();

        public List<SelectListItem> SenderNumberOptions { get; set; } = new();
        public List<SelectListItem> StatusOptions { get; set; } = new();
        public List<SelectListItem> ChannelOptions { get; set; } = new();
    }
}
