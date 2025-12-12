using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Project.SenderWallet
{
    public class SenderWalletTransferViewModel
    {
        public SenderWalletTransferDTO TransferModel { get; set; }
            = new SenderWalletTransferDTO();

        public string FromSenderNumberText { get; set; }

        public List<SelectListItem> SenderNumberOptions { get; set; }
            = new List<SelectListItem>();
    }
}
