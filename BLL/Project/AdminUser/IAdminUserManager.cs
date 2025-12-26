using DTO.Base;
using DTO.DataTable;
using DTO.Project.AdminUser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Project.AdminUser
{
    public interface IAdminUserManager
    {
        /// <summary>
        /// گرفتن تمام کاربران مدیر
        /// </summary>
        List<AdminUserSummaryDTO> GetAll();

        /// <summary>
        /// خروجی مناسب دیتاتیبل
        /// </summary>
        DataTableResponseDTO<AdminUserSummaryDTO> GetDataTableDTO(DataTableSearchDTO search);

        /// <summary>
        /// دریافت جزئیات کاربر مدیر
        /// </summary>
        AdminUserDetailDTO GetById(string id);

        /// <summary>
        /// ایجاد کاربر مدیر جدید
        /// </summary>
        BaseResult Create(AdminUserCreateDTO model);

        /// <summary>
        /// ویرایش اطلاعات کاربر مدیر
        /// </summary>
        BaseResult Update(AdminUserEditDTO model);

        /// <summary>
        /// حذف کاربر مدیر
        /// </summary>
        BaseResult Delete(string id);
    }
}
