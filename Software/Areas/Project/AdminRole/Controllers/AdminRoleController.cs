using BLL.Project.AdminRole;
using BLL.Project.RolePermission;
using DTO.Base;
using DTO.DataTable;
using DTO.Project.AdminRole;
using DTO.Project.RolePermission;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace PanelSMS.Areas.Project.AdminRole.Controllers
{
    [Area("Project")]
    public class AdminRoleController : Controller
    {
        private readonly IAdminRoleManager _roleManager;
        private readonly IRolePermissionManager _rolePermissionManager;

        public AdminRoleController(IAdminRoleManager roleManager, IRolePermissionManager rolePermissionManager)
        {
            _roleManager = roleManager;
            _rolePermissionManager = rolePermissionManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> GetList()
        {
            var search = new DataTableSearchDTO
            {
                start = int.Parse(Request.Form["start"]),
                length = int.Parse(Request.Form["length"]),
                draw = Request.Form["draw"],
                searchValue = Request.Form["search[value]"]
            };

            var colIndex = Request.Form["order[0][column]"];
            search.sortDirection = Request.Form["order[0][dir]"];
            search.sortColumnName = Request.Form[$"columns[{colIndex}][data]"];

            var result = await _roleManager.GetDataTableDTOAsync(search);
            return Json(result);
        }

        [HttpGet]
        public async Task<IActionResult> LoadCreateForm()
        {
            var viewModel = new AdminRoleFormViewModel
            {
                CreateModel = new AdminRoleCreateDTO(),
                StatusOptions = GetStatusOptions()
            };

            return PartialView("_Create", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AdminRoleFormViewModel model)
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

            // بررسی یکتایی نام
            var isNameUnique = await _roleManager.IsNameUniqueAsync(model.CreateModel.Name);
            if (!isNameUnique)
            {
                return Json(new BaseResult
                {
                    Status = false,
                    Message = "این نام نقش قبلاً ثبت شده است."
                });
            }

            var result = await _roleManager.CreateAsync(model.CreateModel);
            return Json(result);
        }

        [HttpGet]
        public async Task<IActionResult> LoadEditForm(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return Content("شناسه نقش نامعتبر است.");

            var editModel = await _roleManager.GetByIdAsync(id);
            if (editModel == null)
                return Content("نقش مورد نظر یافت نشد.");

            var viewModel = new AdminRoleFormViewModel
            {
                EditModel = editModel,
                StatusOptions = GetStatusOptions()
            };

            return PartialView("_Edit", viewModel);
        }

        [HttpPut]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AdminRoleFormViewModel model)
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

            // بررسی یکتایی نام (به جز نقش فعلی)
            var isNameUnique = await _roleManager.IsNameUniqueAsync(
                model.EditModel.Name,
                model.EditModel.Id);

            if (!isNameUnique)
            {
                return Json(new BaseResult
                {
                    Status = false,
                    Message = "این نام نقش قبلاً ثبت شده است."
                });
            }

            var result = await _roleManager.UpdateAsync(model.EditModel);
            return Json(result);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return Json(new BaseResult
                {
                    Status = false,
                    Message = "شناسه نقش نامعتبر است."
                });
            }

            var result = await _roleManager.DeleteAsync(id);
            return Json(result);
        }

        [HttpPut]
        public async Task<IActionResult> ToggleActive(string id, bool isActive)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return Json(new BaseResult
                {
                    Status = false,
                    Message = "شناسه نقش نامعتبر است."
                });
            }

            var result = await _roleManager.ToggleActiveAsync(id, isActive);
            return Json(result);
        }

        [HttpGet]
        public async Task<IActionResult> CheckNameUnique(string name, string excludeId = null)
        {
            var isUnique = await _roleManager.IsNameUniqueAsync(name, excludeId);
            return Json(new { isUnique });
        }

        [HttpGet]
        public async Task<IActionResult> GetRolesDropdown()
        {
            var roles = await _roleManager.GetActiveRolesForDropdownAsync();
            return Json(roles);
        }

        // Helper Methods
        private List<SelectListItem> GetStatusOptions()
        {
            return new List<SelectListItem>
            {
                new SelectListItem { Value = "true", Text = "فعال" },
                new SelectListItem { Value = "false", Text = "غیرفعال" }
            };
        }


        [HttpGet]
        public async Task<IActionResult> GetRolePermissionsSummary(string roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName))
                return Json(new { hasPermissions = false, count = 0 });

            try
            {
                var permissions = await _rolePermissionManager.GetRolePermissionsAsync(null, roleName);
                return Json(new
                {
                    hasPermissions = permissions.TotalAssignedActions > 0,
                    count = permissions.TotalAssignedActions
                });
            }
            catch
            {
                return Json(new { hasPermissions = false, count = 0 });
            }
        }



        /* ------------ Role Permissions ------------ */

        public async Task<IActionResult> LoadPermissionsForm(string id)
        {
            // ابتدا نقش را بگیر برای دریافت نام
            var roleData = await _roleManager.GetByIdAsync(id);
            if (roleData == null)
                return Content("خطا در دریافت اطلاعات نقش.");

            // سپس مجوزهای نقش را بگیر
            var permissionsData = _rolePermissionManager.GetRolePermissions(roleData.Name);
            if (permissionsData == null)
                return Content("خطا در دریافت مجوزهای نقش.");

            permissionsData.RoleId = id;
            return PartialView("_RolePermissions", permissionsData);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public IActionResult SavePermissions([FromBody] SavePermissionsRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request?.RoleName) || request.Permissions == null)
                {
                    return Json(new BaseResult
                    {
                        Status = false,
                        Message = "اطلاعات ارسال‌شده معتبر نیست."
                    });
                }

                var result = _rolePermissionManager.SaveRolePermissions(request.RoleName, request.Permissions);
                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(new BaseResult { Status = false, Message = ex.Message });
            }
        }

        public class SavePermissionsRequest
        {
            public string RoleId { get; set; }
            public string RoleName { get; set; }
            public List<RoutePermissionInputDTO> Permissions { get; set; }
        }
    }
}
