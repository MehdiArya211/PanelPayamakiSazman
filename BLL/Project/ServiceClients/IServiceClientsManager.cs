using DTO.Base;
using DTO.DataTable;
using DTO.Project.ServiceClients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Project.ServiceClients
{
    public interface IServiceClientsManager
    {
        DataTableResponseDTO<ServiceClientListDTO> GetDataTable(DataTableSearchDTO search);

        ServiceClientDetailDTO GetById(string clientId);

        BaseResult Create(ServiceClientCreateDTO model);

        BaseResult RotateSecret(string clientId, out string newSecret);

        BaseResult ChangeStatus(string clientId, bool isActive);
    }
}
