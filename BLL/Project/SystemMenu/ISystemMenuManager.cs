using DTO.Base;
using DTO.DataTable;
using DTO.Project.SystemMenu;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Project.SystemMenu
{
    public interface ISystemMenuManager
    {
        /// <summary>
        /// جستجو جهت DataTable
        /// </summary>
        DataTableResponseDTO<SystemMenuListDTO> GetDataTable(DataTableSearchDTO search);

        /// <summary>
        /// دریافت همه منوها
        /// </summary>
        List<SystemMenuListDTO> GetAll();

        /// <summary>
        /// ایجاد منو جدید
        /// </summary>
        BaseResult Create(SystemMenuCreateDTO model);

        /// <summary>
        /// دریافت یک منو
        /// </summary>
        SystemMenuEditDTO GetById(string id);

        /// <summary>
        /// ویرایش منو
        /// </summary>
        BaseResult Update(SystemMenuEditDTO model);

        /// <summary>
        /// حذف منو
        /// </summary>
        BaseResult Delete(string id);

        public List<SelectListItem> GetParentMenusForDropdown();

    }
}

