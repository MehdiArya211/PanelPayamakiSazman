using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Project.SenderWallet
{
    public class WalletTransactionDTO
    {
        public string Id { get; set; }
        public decimal Amount { get; set; }
        public bool IsCredit { get; set; }

        /// <summary>
        /// نوع تراکنش: ProvisionalDebit / ... (طبق enum سرویس)
        /// </summary>
        public string Type { get; set; }

        public string Reference { get; set; }
        public string Description { get; set; }
        public DateTime? CreatedOn { get; set; }
    }
}
