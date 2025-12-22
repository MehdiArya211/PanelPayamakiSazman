using DTO.Base;
using DTO.Project.RoutePermission;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interface
{
    public interface IRoutePermissionManager
    {
        /// <summary>
        /// گرفتن همه مجوزها (Async)
        /// </summary>
        Task<List<RoutePermissionListDTO>> GetAllAsync();

        /// <summary>
        /// دریافت اطلاعات یک مجوز برای ویرایش (Async)
        /// </summary>
        Task<RoutePermissionEditDTO> GetByRouteKeyAsync(string routeKey);

        /// <summary>
        /// ویرایش مجوز (Async)
        /// </summary>
        Task<BaseResult> UpdateAsync(RoutePermissionEditDTO model);

        /// <summary>
        /// حذف مجوز (Async)
        /// </summary>
        Task<BaseResult> DeleteAsync(string routeKey);

        /// <summary>
        /// گرفتن لیست Actionهای موجود در سیستم (Async)
        /// </summary>
        Task<List<string>> GetSystemActionsAsync();

        /// <summary>
        /// جستجوی مجوزها (Async)
        /// </summary>
        Task<List<RoutePermissionListDTO>> SearchAsync(string searchTerm);
    }
}
