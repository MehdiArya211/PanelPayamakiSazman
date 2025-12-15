using DTO.Base;
using DTO.DataTable;
using DTO.Project.Unit;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Project.Unit
{
    public interface IUnitManager
    {
        /// <summary>
        /// گرفتن لیست واحدها برای سناریوهای ساده (مثلاً برای dropdown)
        /// </summary>
        List<UnitListDTO> GetAll();

        /// <summary>
        /// گرفتن دیتا به فرمت مناسب DataTable (سرور-ساید شبیه‌سازی شده)
        /// </summary>
        DataTableResponseDTO<UnitListDTO> GetDataTableDTO(DataTableSearchDTO search);

        /// <summary>
        /// ساخت واحد سازمانی
        /// </summary>
        BaseResult Create(UnitCreateDTO model);

        /// <summary>
        /// گرفتن اطلاعات یک واحد برای ویرایش
        /// (چون GET واحد نداری، از search + فیلتر id استفاده می‌کنیم)
        /// </summary>
        UnitEditDTO? GetById(string id);

        /// <summary>
        /// ویرایش واحد
        /// </summary>
        BaseResult Update(UnitEditDTO model);

        public List<SelectListItem> GetUnitsForDropdown();

    }
}
