using DTO.Base;
using DTO.DataTable;
using DTO.Project.SmsConsumer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Project.SmsConsumer
{
    public interface ISmsConsumerManager
    {
        DataTableResponseDTO<SmsMessageHistoryListDTO> GetHistoryDataTableDTO(DataTableSearchDTO search, bool onlyOutbound);

        BaseResult SendSingle(SmsSendSingleDTO model);
        BaseResult SendBulk(string senderNumberId, List<string> receiverNumbers, string text);
        BaseResult SendGroups(SmsSendGroupsDTO model);
        BaseResult SendFile(SmsSendFileDTO model);

        // برای UI
        List<SenderNumberOptionDTO> GetSenderNumberOptions(); // از /api/consumer/sender-numbers/search
        List<ContactGroupOptionDTO> GetContactGroupOptions(); // از /api/consumer/contact-groups/search
    }

    public class SenderNumberOptionDTO
    {
        public string Id { get; set; }
        public string Display { get; set; }
    }

    public class ContactGroupOptionDTO
    {
        public string Id { get; set; }
        public string Display { get; set; }
    }
}
