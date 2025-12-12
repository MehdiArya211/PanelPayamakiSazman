using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Project.SmsConsumer
{
    public class SmsMessageHistoryListDTO
    {
        public string Id { get; set; }
        public string BatchId { get; set; }

        public string SenderNumberId { get; set; }

        public string ReceiverNumber { get; set; }
        public string Text { get; set; }

        public string Status { get; set; }     // Pending, Sent, Delivered, Failed,... (هرچی API بده)
        public string Direction { get; set; }  // Outbound/Inbound
        public string Channel { get; set; }    // Portal,...

        public DateTime? CreatedOn { get; set; }
        public DateTime? ExportedOn { get; set; }
        public DateTime? SentOn { get; set; }
        public DateTime? DeliveredOn { get; set; }

        public decimal? ReservedCost { get; set; }
        public decimal? FinalCost { get; set; }

        public string? FailureReason { get; set; }

        public string? ContactGroupId { get; set; }
        public string? ContactId { get; set; }
        public int? FileRowNumber { get; set; }

       
        public string? SenderFullNumber { get; set; }
    }
}
