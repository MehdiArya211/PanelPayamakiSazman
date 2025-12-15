using DTO.Base;
using DTO.DataTable;
using DTO.Project.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Project.Menus
{
    public interface IAdminMenuManager
    {
        // لیست درختی منوها
        List<MenuTreeNodeDTO> GetTree();

        // لیست صفحه‌بندی شده برای دیتاتیبل
        DataTableResponseDTO<MenuListDTO> GetDataTableDTO(DataTableSearchDTO search);

        // لیست ساده همه منوها برای دراپ‌داون
        List<MenuListDTO> GetAll();

        // اطلاعات یک منو
        MenuEditDTO GetById(string id);

        // ایجاد منوی جدید
        BaseResult Create(MenuCreateDTO model);

        // ویرایش منو
        BaseResult Update(MenuEditDTO model);

        // حذف منو
        BaseResult Delete(string id);
    }
}
