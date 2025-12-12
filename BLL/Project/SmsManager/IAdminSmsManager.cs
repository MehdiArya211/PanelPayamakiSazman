using DTO.Base;
using DTO.DataTable;
using DTO.Project.SmsManager;
using DTO.Project.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Project.SmsManager
{
    public interface IAdminSmsManager
    {
        DataTableResponseDTO<AdminSmsMessageListDTO> GetHistory(AdminSmsHistoryFilterDTO filter, DataTableSearchDTO search);

        BaseResult SendSingle(SmsSingleSendDTO model);
        BaseResult SendBulk(SmsBulkSendDTO model);
        BaseResult SendToGroups(SmsGroupSendDTO model);
        BaseResult SendFromFile(SmsFileSendViewModel model);

        /// <summary>
        /// سرشماره‌ها برای دراپ‌داون
        /// </summary>
        List<LookupItemDTO> GetSenderNumberLookup();

        /// <summary>
        /// گروه‌های مخاطب برای دراپ‌داون (URL را باید بر اساس مستند واقعی اصلاح کنی)
        /// </summary>
        List<LookupItemDTO> GetContactGroupLookup();
    }
}
