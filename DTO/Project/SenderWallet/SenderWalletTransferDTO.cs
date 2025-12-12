using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Project.SenderWallet
{
    public class SenderWalletTransferDTO
    {
        [Required]
        public string FromSenderNumberId { get; set; }

        [Required]
        public string ToSenderNumberId { get; set; }

        [Required(ErrorMessage = "مبلغ الزامی است.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "مبلغ باید بزرگ‌تر از صفر باشد.")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "توضیحات الزامی است.")]
        [StringLength(500, ErrorMessage = "توضیحات نمی‌تواند بیش از 500 کاراکتر باشد.")]
        public string Description { get; set; }
    }
}
