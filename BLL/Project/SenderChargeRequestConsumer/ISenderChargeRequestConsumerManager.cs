using DTO.Base;
using DTO.DataTable;
using DTO.Project.SenderChargeRequestConsumer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Project.SenderChargeRequestConsumer
{
    public interface ISenderChargeRequestConsumerManager
    {
        DataTableResponseDTO<SenderChargeRequestListDTO> GetDataTableDTO(DataTableSearchDTO search);

        BaseResult Create(SenderChargeRequestCreateDTO model);

        List<SenderNumberOptionDTO> GetSenderNumberOptions();
    }
}
