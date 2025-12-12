using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Project.SenderChargeRequestConsumer
{
    public class SenderChargeRequestCreateDTO
    {
        [Required(ErrorMessage = "سرشماره الزامی است.")]
        public string SenderNumberId { get; set; }

        [Required(ErrorMessage = "مبلغ الزامی است.")]
        [Range(1, double.MaxValue, ErrorMessage = "مبلغ باید بزرگ‌تر از صفر باشد.")]
        public double Amount { get; set; }

        [Required(ErrorMessage = "شرح پرداخت الزامی است.")]
        [StringLength(500, ErrorMessage = "شرح پرداخت بیش از حد طولانی است.")]
        public string PaymentDescription { get; set; }

        public string? BankAccountNumber { get; set; }

        public IFormFile? ReceiptImage { get; set; }
    }
}
