using DTO.Base;
using DTO.DataTable;
using DTO.Project.SenderChargeRequest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Project.SenderChargeRequest
{
    public interface IChargeRequestManager
    {
        /// <summary>
        /// گرفتن همه لیست (در صورت نیاز)
        /// </summary>
        List<ChargeRequestListDTO> GetAll();

        /// <summary>
        /// خروجی مناسب DataTable
        /// </summary>
        DataTableResponseDTO<ChargeRequestListDTO> GetDataTableDTO(DataTableSearchDTO search);

        /// <summary>
        /// تایید درخواست شارژ
        /// </summary>
        BaseResult Approve(string id, string note);

        /// <summary>
        /// رد درخواست شارژ
        /// </summary>
        BaseResult Reject(string id, string note);
    }
}
