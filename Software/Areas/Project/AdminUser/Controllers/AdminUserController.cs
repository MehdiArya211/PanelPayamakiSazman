using BLL.Project.AdminUser;
using DTO.DataTable;
using DTO.Project.AdminUser;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace PanelSMS.Areas.Project.AdminUser.Controllers
{
    [Area("Project")]
    public class AdminUserController : Controller
    {
        private readonly IAdminUserManager _adminUserManager;

        public AdminUserController(IAdminUserManager adminUserManager)
        {
            _adminUserManager = adminUserManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public IActionResult GetList()
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

            var result = _adminUserManager.GetDataTableDTO(search);
            return Json(result);
        }

        /* ------------ Create ------------ */

        public IActionResult LoadCreateForm()
        {
            var model = new AdminUserFormViewModel
            {
                CreateModel = new AdminUserCreateDTO(),
                UnitOptions = new List<SelectListItem>
                {
                    new SelectListItem { Value = "1", Text = "واحد اصلی" },
                    new SelectListItem { Value = "2", Text = "واحد فرعی" }
                },
                RoleOptions = new List<SelectListItem>
                {
                    new SelectListItem { Value = "1", Text = "مدیر" },
                    new SelectListItem { Value = "2", Text = "کاربر عادی" }
                }
            };

            return PartialView("_Create", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(AdminUserFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new
                {
                    status = false,
                    message = "اطلاعات ارسال‌شده معتبر نیست!"
                });
            }

            var res = _adminUserManager.Create(model.CreateModel);
            return Json(res);
        }

        /* ------------ Edit ------------ */

        public IActionResult LoadEditForm(string id)
        {
            var adminUserDetail = _adminUserManager.GetById(id);
            if (adminUserDetail == null)
                return Content("خطا در دریافت اطلاعات کاربر.");

            var model = new AdminUserFormViewModel
            {
                EditModel = new AdminUserEditDTO
                {
                    Id = adminUserDetail.Id,
                    UserName = adminUserDetail.UserName,
                    FirstName = adminUserDetail.FirstName,
                    LastName = adminUserDetail.LastName,
                    NationalCode = adminUserDetail.NationalCode,
                    MobileNumber = adminUserDetail.MobileNumber,
                    UnitId = adminUserDetail.UnitId,
                    CurrentRoles = adminUserDetail.Roles
                },
                UnitOptions = new List<SelectListItem>
                {
                    new SelectListItem { Value = "1", Text = "واحد اصلی" },
                    new SelectListItem { Value = "2", Text = "واحد فرعی" }
                },
                RoleOptions = new List<SelectListItem>
                {
                    new SelectListItem { Value = "1", Text = "مدیر" },
                    new SelectListItem { Value = "2", Text = "کاربر عادی" }
                }
            };

            return PartialView("_Edit", model);
        }

        [HttpPut]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(AdminUserFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new
                {
                    status = false,
                    message = "اطلاعات ارسال‌شده معتبر نیست!"
                });
            }

            var res = _adminUserManager.Update(model.EditModel);
            return Json(res);
        }

        [HttpPost]
        public IActionResult Delete(string id)
        {
            var res = _adminUserManager.Delete(id);
            return Json(res);
        }
    }
}
