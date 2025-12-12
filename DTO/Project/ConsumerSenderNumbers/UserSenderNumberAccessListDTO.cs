using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Project.ConsumerSenderNumbers
{
    // ردیف‌های لیست سرشماره‌های مجاز کاربر
    public class UserSenderNumberAccessListDTO
    {
        public string Id { get; set; }                 // احتمالاً accessId یا senderNumberId (طبق API)
        public string SenderNumberId { get; set; }      // اگر وجود دارد
        public string FullNumber { get; set; }          // سرشماره کامل
        public string FixedPrefix { get; set; }         // پیش‌شماره ثابت
        public string Status { get; set; }              // وضعیت سرشماره
        public decimal? WalletBalance { get; set; }     // موجودی کیف پول
        public bool? IsServiceClient { get; set; }      // اگر داشت
        public string? Description { get; set; }
    }
}
