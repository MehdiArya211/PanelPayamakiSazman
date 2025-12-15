using BLL.Project.SenderNumberSubArea;
using BLL.Project.SystemRole;
using BLL.Project.Unit;
using BLL.Project.User;
using DTO.DataTable;
using DTO.Project.User;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace PanelSMS.Areas.Project.User.Controllers
{
    [Area("Project")]

    public class SystemUserController : Controller
    {
        private readonly ISystemUserManager _userManager;
        private readonly IUnitManager _unitManager;
        private readonly ISystemRoleManager _systemRoleManager;


        public SystemUserController(ISystemUserManager userManager, IUnitManager unitManager,
            ISystemRoleManager systemRoleManager)
        {
            _userManager = userManager;
            _unitManager = unitManager;
            _systemRoleManager = systemRoleManager;
        }
        #region نمایش
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult GetList()
        {
            var search = new DataTableSearchDTO();

            search.start = int.Parse(Request.Form["start"]);
            search.length = int.Parse(Request.Form["length"]);
            search.draw = Request.Form["draw"];

            var colIndex = Request.Form["order[0][column]"];
            search.sortDirection = Request.Form["order[0][dir]"];
            search.sortColumnName = Request.Form[$"columns[{colIndex}][data]"];

            var result = _userManager.GetDataTable(search);

            return Json(result);
        }
        #endregion

        #region ایجاد
        public IActionResult LoadCreateForm()
        {
            var unitList = _unitManager.GetUnitsForDropdown();
            var roleList = _systemRoleManager.GetRoleLookup();

            ViewBag.Units = unitList
                .Select(x => new SelectListItem
                {
                    Value = x.Value,
                    Text = x.Text
                })
                .ToList();

            ViewBag.Roles = roleList
                .Select(x => new SelectListItem
                {
                    Value = x.Id,
                    Text = x.Text
                })
                .ToList();
            return PartialView("_Create", new SystemUserCreateDTO());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public IActionResult Create(SystemUserCreateDTO model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new
                {
                    status = false,
                    message = "اطلاعات ارسال‌شده معتبر نیست!"
                });
            }
            var res = _userManager.Create(model);
            return Json(res);
        }
        #endregion


        public IActionResult LoadEditForm(string id)
        {
            var model = _userManager.GetById(id);
            return PartialView("_Edit", model);
        }

        [HttpPut]
        [ValidateAntiForgeryToken]

        public IActionResult Edit(SystemUserEditDTO model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new
                {
                    status = false,
                    message = "اطلاعات ارسال‌شده معتبر نیست!"
                });
            }
            var res = _userManager.Update(model);
            return Json(res);
        }

        [HttpPost]
        public IActionResult Delete(string id)
        {
            var res = _userManager.Delete(id);
            return Json(res);
        }

        [HttpGet]
        public IActionResult GetAllForSelect()
        {
            var search = new DataTableSearchDTO
            {
                start = 0,
                length = 1000,
                draw = "1",
                sortColumnName = "UserName",
                sortDirection = "asc"
            };

            var result = _userManager.GetDataTable(search);

            // فقط دیتا برای select
            return Json(result.data);
        }


    }
}
