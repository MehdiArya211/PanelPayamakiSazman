using DTO.Base;
using DTO.DataTable;
using DTO.Project.SenderWallet;
using DTO.Project.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Project.SenderWallet
{
    public interface ISenderWalletManager
    {
        /// <summary>
        /// گرفتن کیف پول یک سرشماره
        /// </summary>
        SenderWalletDTO GetWallet(string senderNumberId);

        /// <summary>
        /// لیست تراکنش‌های کیف پول (خروجی مناسب DataTable)
        /// </summary>
        DataTableResponseDTO<WalletTransactionDTO> GetTransactions(string senderNumberId, DataTableSearchDTO search);

        /// <summary>
        /// شارژ کیف پول
        /// </summary>
        BaseResult Charge(SenderWalletChargeDTO model);

        /// <summary>
        /// انتقال موجودی بین دو کیف پول سرشماره
        /// </summary>
        BaseResult Transfer(SenderWalletTransferDTO model);

        /// <summary>
        /// لیست سرشماره‌ها برای دراپ‌داون (fullNumber و ...).
        /// </summary>
        List<LookupItemDTO> GetSenderNumberLookup();
    }
}
