using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Project.SmsManager
{
    /// <summary>
    /// DTO نمایش لاگ پیامک‌ها در ادمین
    /// </summary>
    public class AdminSmsMessageListDTO
    {
        public string Id { get; set; }
        public string BatchId { get; set; }

        public string SenderNumberId { get; set; }
        public string SenderNumber { get; set; }

        public string RequestedByUserId { get; set; }
        public string RequestedByUserName { get; set; }
        public string RequestedByFullName { get; set; }

        public string ReceiverNumber { get; set; }
        public string Text { get; set; }

        public string Status { get; set; }      // Pending / Sent / Delivered / Failed ...
        public string Direction { get; set; }   // Outbound / Inbound
        public string Channel { get; set; }     // Portal / Api / ...

        public DateTime? CreatedOn { get; set; }
        public DateTime? ExportedOn { get; set; }
        public DateTime? SentOn { get; set; }
        public DateTime? DeliveredOn { get; set; }

        public decimal? ReservedCost { get; set; }
        public decimal? FinalCost { get; set; }

        public string FailureReason { get; set; }
    }
}
