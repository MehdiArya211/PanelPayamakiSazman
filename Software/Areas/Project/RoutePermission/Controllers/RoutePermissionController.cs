using BLL.Interface;
using DTO.Base;
using DTO.DataTable;
using DTO.Project.RoutePermission;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace PanelSMS.Areas.Project.RoutePermission.Controllers
{
    [Area("Project")]
    public class RoutePermissionController : Controller
    {
        private readonly IRoutePermissionManager _permissionManager;

        public RoutePermissionController(IRoutePermissionManager permissionManager)
        {
            _permissionManager = permissionManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GetList([FromBody] DataTableSearchDTO search)
        {
            try
            {
                var allPermissions = await _permissionManager.GetAllAsync();

                // اعمال جستجو
                if (!string.IsNullOrWhiteSpace(search.searchValue))
                {
                    var searchTerm = search.searchValue.Trim().ToLower();
                    allPermissions = allPermissions
                        .Where(p =>
                            (p.RouteKey?.ToLower().Contains(searchTerm) ?? false) ||
                            (p.DisplayActions?.ToLower().Contains(searchTerm) ?? false))
                        .ToList();
                }

                // اعمال مرتب‌سازی
                if (!string.IsNullOrWhiteSpace(search.sortColumnName))
                {
                    allPermissions = search.sortDirection == "desc"
                        ? allPermissions.OrderByDescending(GetPropertyValue(search.sortColumnName)).ToList()
                        : allPermissions.OrderBy(GetPropertyValue(search.sortColumnName)).ToList();
                }
                else
                {
                    allPermissions = allPermissions.OrderBy(p => p.RouteKey).ToList();
                }

                // اعمال صفحه‌بندی
                var totalRecords = allPermissions.Count;
                var data = allPermissions
                    .Skip(search.start)
                    .Take(search.length)
                    .ToList();

                return Json(new DataTableResponseDTO<RoutePermissionListDTO>
                {
                    draw = search.draw,
                    recordsTotal = totalRecords,
                    recordsFiltered = totalRecords,
                    data = data
                });
            }
            catch (Exception ex)
            {
                return Json(new DataTableResponseDTO<RoutePermissionListDTO>
                {
                    draw = search.draw,
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = new List<RoutePermissionListDTO>(),
                    AdditionalData = ex.Message
                });
            }
        }

        private Func<RoutePermissionListDTO, object> GetPropertyValue(string propertyName)
        {
            return propertyName.ToLower() switch
            {
                "routekey" => p => p.RouteKey,
                "actions" => p => p.DisplayActions,
                "requiresfreshauth" => p => p.RequiresFreshAuthText,
                _ => p => p.RouteKey
            };
        }

        [HttpGet]
        public async Task<IActionResult> LoadEditForm(string routeKey)
        {
            if (string.IsNullOrWhiteSpace(routeKey))
                return Content("Route Key الزامی است.");

            var editModel = await _permissionManager.GetByRouteKeyAsync(routeKey);
            if (editModel == null)
                return Content("مجوز مورد نظر یافت نشد.");

            var systemActions = await _permissionManager.GetSystemActionsAsync();

            var viewModel = new RoutePermissionFormViewModel
            {
                EditModel = editModel,
                AvailableActions = systemActions.Select(a => new SelectListItem
                {
                    Value = a,
                    Text = a,
                    Selected = editModel.Actions?.Contains(a) ?? false
                }).ToList(),
                FreshAuthOptions = new List<SelectListItem>
                {
                    new SelectListItem { Value = "true", Text = "بله", Selected = editModel.RequiresFreshAuth },
                    new SelectListItem { Value = "false", Text = "خیر", Selected = !editModel.RequiresFreshAuth }
                }
            };

            return PartialView("_Edit", viewModel);
        }

        [HttpPut]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(RoutePermissionFormViewModel model)
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

            var result = await _permissionManager.UpdateAsync(model.EditModel);
            return Json(result);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(string routeKey)
        {
            if (string.IsNullOrWhiteSpace(routeKey))
            {
                return Json(new BaseResult
                {
                    Status = false,
                    Message = "Route Key الزامی است."
                });
            }

            var result = await _permissionManager.DeleteAsync(routeKey);
            return Json(result);
        }

        [HttpGet]
        public async Task<IActionResult> Search(string term)
        {
            if (string.IsNullOrWhiteSpace(term))
                return Json(new List<RoutePermissionListDTO>());

            var results = await _permissionManager.SearchAsync(term);
            return Json(results);
        }

        [HttpGet]
        public async Task<IActionResult> GetPermissionsDropdown()
        {
            var permissions = await _permissionManager.GetAllAsync();
            var dropdownItems = permissions.Select(p => new
            {
                Value = p.RouteKey,
                Text = p.RouteKey
            }).ToList();

            return Json(dropdownItems);
        }
    }
}
