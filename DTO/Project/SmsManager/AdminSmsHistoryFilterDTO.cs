using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Project.SmsManager
{
    /// <summary>
    /// فیلترهای فرم جستجو در پنل ادمین
    /// </summary>
    public class AdminSmsHistoryFilterDTO
    {
        public string Search { get; set; }
        public string SenderNumberId { get; set; }
        public string BatchId { get; set; }
        public string RequestedByUserId { get; set; }
        public string ExcludeRequestedByUserId { get; set; }

        public string Status { get; set; }   // Pending, Sent, Delivered, Failed, ...
        public string Channel { get; set; }  // Portal, Api, ...

        public DateTime? From { get; set; }
        public DateTime? To { get; set; }

        public bool OnlyOutbound { get; set; } = true;
    }
}
