using DTO.Base;
using DTO.DataTable;
using DTO.Project.SenderNumberOrganizationLevel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Project.SenderNumberOrganizationLevel
{
    public interface ISenderNumberOrganizationLevelManager
    {
        /// <summary>
        /// گرفتن همه لیست (برای دراپ‌داون و ...)
        /// </summary>
        List<SenderNumberOrganizationLevelListDTO> GetAll();

        /// <summary>
        /// خروجی مناسب DataTable
        /// </summary>
        DataTableResponseDTO<SenderNumberOrganizationLevelListDTO> GetDataTableDTO(DataTableSearchDTO search);

        /// <summary>
        /// ایجاد رده سازمانی
        /// </summary>
        BaseResult Create(SenderNumberOrganizationLevelCreateDTO model);

        /// <summary>
        /// اطلاعات یک رده برای ویرایش
        /// </summary>
        SenderNumberOrganizationLevelEditDTO GetById(string id);

        /// <summary>
        /// ویرایش رده سازمانی
        /// </summary>
        BaseResult Update(SenderNumberOrganizationLevelEditDTO model);

        /// <summary>
        /// حذف رده سازمانی
        /// </summary>
        BaseResult Delete(string id);
    }
}
