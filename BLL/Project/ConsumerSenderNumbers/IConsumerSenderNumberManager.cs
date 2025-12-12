using DTO.DataTable;
using DTO.Project.ConsumerSenderNumbers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Project.ConsumerSenderNumbers
{
    public interface IConsumerSenderNumberManager
    {
        DataTableResponseDTO<UserSenderNumberAccessListDTO> GetDataTableDTO(DataTableSearchDTO search);

        // برای dropdown ارسال پیامک (id + fullNumber)
        List<SenderNumberOptionDTO> GetOptions();
    }

    public class SenderNumberOptionDTO
    {
        public string Id { get; set; }
        public string Display { get; set; } // fullNumber
    }
}
