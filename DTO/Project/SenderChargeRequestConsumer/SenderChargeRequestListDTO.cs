using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Project.SenderChargeRequestConsumer
{
    public class SenderChargeRequestListDTO
    {
        public string Id { get; set; }
        public string WalletId { get; set; }
        public string SenderNumberId { get; set; }
        public string RequestedByUserId { get; set; }

        public double Amount { get; set; }
        public string Status { get; set; }              // Pending / Approved / Rejected (طبق بک‌اند شما)
        public string PaymentDescription { get; set; }

        public string? ReceiptImageUrl { get; set; }
        public string? BankAccountNumber { get; set; }
        public string? ReviewNote { get; set; }
        public string? CreatedOn { get; set; }

        public string? SenderNumberFullNumber { get; set; }
        public string? SenderNumberFixedPrefix { get; set; } // اختیاری
    }
}
