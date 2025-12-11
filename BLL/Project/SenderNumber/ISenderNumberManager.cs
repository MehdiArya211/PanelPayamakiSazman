using DTO.Base;
using DTO.DataTable;
using DTO.Project.SenderNumber;
using DTO.Project.SenderNumberOrganizationLevel;
using DTO.Project.SenderNumberSpeciality;
using DTO.Project.SenderNumberSubAreaList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Project.SenderNumber
{
    public interface ISenderNumberManager
    {
        /// <summary>
        /// گرفتن همه سرشماره‌ها (مصارف عمومی)
        /// </summary>
        List<SenderNumberListDTO> GetAll();

        /// <summary>
        /// خروجی مناسب دیتاتیبل
        /// </summary>
        DataTableResponseDTO<SenderNumberListDTO> GetDataTableDTO(DataTableSearchDTO search);

        /// <summary>
        /// دریافت اطلاعات یک سرشماره برای ویرایش
        /// </summary>
        SenderNumberEditDTO GetById(string id);

        /// <summary>
        /// ایجاد سرشماره جدید
        /// </summary>
        BaseResult Create(SenderNumberCreateDTO model);

        /// <summary>
        /// ویرایش سرشماره
        /// </summary>
        BaseResult Update(SenderNumberEditDTO model);

        /// <summary>
        /// حذف سرشماره
        /// </summary>
        BaseResult Delete(string id);

        /// <summary>
        /// لیست حوزه‌های تخصصی سرشماره برای دراپ‌داون
        /// </summary>
        List<SenderNumberSpecialityListDTO> GetAllSpecialities();

        /// <summary>
        /// لیست رده‌های سازمانی سرشماره برای دراپ‌داون
        /// </summary>
        List<SenderNumberOrganizationLevelListDTO> GetAllOrganizationLevels();

        /// <summary>
        /// لیست حوزه‌های فرعی سرشماره برای دراپ‌داون
        /// </summary>
        List<SenderNumberSubAreaListDTO> GetAllSubAreas();


        BaseResult UpdateStatus(string id, string status);
    }
}
