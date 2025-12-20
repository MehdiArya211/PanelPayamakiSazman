using DTO.Base;
using DTO.DataTable;
using DTO.Project.AdminRole;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Project.AdminRole
{
    public interface IAdminRoleManager
    {
        /// <summary>
        /// گرفتن همه نقش‌ها (Async)
        /// </summary>
        Task<List<AdminRoleListDTO>> GetAllAsync();

        /// <summary>
        /// خروجی مناسب دیتاتیبل (Async)
        /// </summary>
        Task<DataTableResponseDTO<AdminRoleListDTO>> GetDataTableDTOAsync(DataTableSearchDTO search);

        /// <summary>
        /// دریافت اطلاعات یک نقش برای ویرایش (Async)
        /// </summary>
        Task<AdminRoleEditDTO> GetByIdAsync(string id);

        /// <summary>
        /// ایجاد نقش جدید (Async)
        /// </summary>
        Task<BaseResult> CreateAsync(AdminRoleCreateDTO model);

        /// <summary>
        /// ویرایش نقش (Async)
        /// </summary>
        Task<BaseResult> UpdateAsync(AdminRoleEditDTO model);

        /// <summary>
        /// حذف نقش (Async)
        /// </summary>
        Task<BaseResult> DeleteAsync(string id);

        /// <summary>
        /// تغییر وضعیت فعال/غیرفعال (Async)
        /// </summary>
        Task<BaseResult> ToggleActiveAsync(string id, bool isActive);

        /// <summary>
        /// بررسی یکتایی نام نقش (Async)
        /// </summary>
        Task<bool> IsNameUniqueAsync(string name, string excludeId = null);

        /// <summary>
        /// گرفتن لیست نقش‌های فعال برای دراپ‌داون (Async)
        /// </summary>
        Task<List<SelectListItem>> GetActiveRolesForDropdownAsync();
    }
}
