using BLL.Project.SenderNumberSubArea;
using BLL.Project.User;
using DTO.DataTable;
using DTO.Project.User;
using Microsoft.AspNetCore.Mvc;

namespace PanelSMS.Areas.Project.User.Controllers
{
    [Area("Project")]

    public class SystemUserController : Controller
    {
        private readonly ISystemUserManager _userManager;

        public SystemUserController(ISystemUserManager userManager)
        {
            _userManager = userManager;
        }

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

        public IActionResult LoadCreateForm()
        {
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
    }
}
