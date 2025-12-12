using DTO.Base;
using DTO.DataTable;
using DTO.Project.SenderNumberAssignment;
using DTO.Project.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Project.SenderNumberAssignment
{
    public interface ISenderNumberAssignmentManager
    {
        List<SenderNumberAssignmentListDTO> GetAll();

        DataTableResponseDTO<SenderNumberAssignmentListDTO> GetDataTableDTO(DataTableSearchDTO search);

        SenderNumberAssignmentEditDTO GetById(string id);

        BaseResult Create(SenderNumberAssignmentCreateDTO model);

        BaseResult Update(SenderNumberAssignmentEditDTO model);

        /// <summary>
        /// لغو دسترسی کاربر از سرشماره
        /// </summary>
        BaseResult Revoke(string id, string reason);


        List<LookupItemDTO> GetSenderNumberLookup();
        List<LookupItemDTO> GetUserLookup();
        List<LookupItemDTO> GetClientLookup();
    }
}
