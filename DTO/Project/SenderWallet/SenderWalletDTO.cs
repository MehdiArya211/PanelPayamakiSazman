using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Project.SenderWallet
{
    public class SenderWalletDTO
    {
        public string WalletId { get; set; }
        public string SenderNumberId { get; set; }
        public decimal Balance { get; set; }
    }
}
