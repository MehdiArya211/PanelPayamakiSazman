using BLL.Project.AdminRole;
using BLL.Project.RolePermission;
using DTO.Base;
using DTO.Project.RolePermission;
using Microsoft.AspNetCore.Mvc;

namespace PanelSMS.Areas.Project.RolePermission.Controllers
{
    [Area("Project")]
    public class RolePermissionController : Controller
    {
        private readonly IRolePermissionManager _permissionManager;
        private readonly IAdminRoleManager _roleManager;

        public RolePermissionController(
            IRolePermissionManager permissionManager,
            IAdminRoleManager roleManager)
        {
            _permissionManager = permissionManager;
            _roleManager = roleManager;
        }

        [HttpGet]
        public async Task<IActionResult> LoadPermissionsModal(string roleId, string roleName)
        {
            if (string.IsNullOrWhiteSpace(roleId) || string.IsNullOrWhiteSpace(roleName))
                return Content("شناسه یا نام نقش نامعتبر است.");

            var viewModel = await _permissionManager.GetRolePermissionsAsync(roleId, roleName);
            if (viewModel == null)
                return Content("خطا در دریافت مجوزهای نقش.");

            return PartialView("_RolePermissions", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SavePermissions([FromBody] RolePermissionBulkUpdateDto model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return Json(new BaseResult
                {
                    Status = false,
                    Message = "اطلاعات ارسال‌شده معتبر نیست!",
                    //Data = errors
                });
            }

            var result = await _permissionManager.SaveRolePermissionsAsync(model);
            return Json(result);
        }

        [HttpPost]
        public async Task<IActionResult> CopyPermissions(string sourceRoleName, string targetRoleName)
        {
            if (string.IsNullOrWhiteSpace(sourceRoleName) || string.IsNullOrWhiteSpace(targetRoleName))
            {
                return Json(new BaseResult
                {
                    Status = false,
                    Message = "نام نقش‌ها الزامی است."
                });
            }

            var result = await _permissionManager.CopyPermissionsAsync(sourceRoleName, targetRoleName);
            return Json(result);
        }

        [HttpPost]
        public async Task<IActionResult> ResetPermissions(string roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName))
            {
                return Json(new BaseResult
                {
                    Status = false,
                    Message = "نام نقش الزامی است."
                });
            }

            var result = await _permissionManager.ResetPermissionsAsync(roleName);
            return Json(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetPermissionsSummary(string roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName))
                return Json(new { totalRoutes = 0, totalActions = 0 });

            var permissions = await _permissionManager.GetRolePermissionsAsync(null, roleName);

            return Json(new
            {
                totalRoutes = permissions.TotalRoutes,
                totalActions = permissions.TotalAssignedActions
            });
        }
    }
}
