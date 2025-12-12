using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Project.SenderChargeRequest
{
    /// <summary>
    /// DTO نمایش درخواست شارژ سرشماره در سمت ادمین
    /// </summary>
    public class ChargeRequestListDTO
    {
        public string Id { get; set; }              // requestId (uuid)
        public string WalletId { get; set; }        // احتمالاً guid سمت سرویس
        public string SenderNumberId { get; set; }  // guid
        public string RequestedByUserId { get; set; }

        public decimal Amount { get; set; }

        /// <summary>
        /// وضعیت درخواست: Pending / Approved / Rejected (احتمالاً)
        /// </summary>
        public string Status { get; set; }

        public string PaymentDescription { get; set; }
        public string ReceiptImageUrl { get; set; }
        public string BankAccountNumber { get; set; }
        public string ReviewNote { get; set; }
        public DateTime? CreatedOn { get; set; }

        // ======== فیلدهای تکمیلی برای نمایش ========

        /// <summary>
        /// سرشماره کامل (FullNumber) مربوط به SenderNumberId
        /// از سرویس سرشماره‌ها خوانده می‌شود.
        /// </summary>
        public string SenderNumberFullNumber { get; set; }

        /// <summary>
        /// عنوان/نام کیف پول – در صورت نیاز از سرویس کیف پول پر شود.
        /// </summary>
        public string WalletTitle { get; set; }

        /// <summary>
        /// نام کاربری درخواست‌دهنده – در صورت نیاز از سرویس کاربران پر شود.
        /// </summary>
        public string RequestedByUserFullName { get; set; }
    }

   
    public class SenderNumberInfoDTO
    {
        public string Id { get; set; }
        public string FullNumber { get; set; }
        public string FixedPrefix { get; set; }
    }
}
