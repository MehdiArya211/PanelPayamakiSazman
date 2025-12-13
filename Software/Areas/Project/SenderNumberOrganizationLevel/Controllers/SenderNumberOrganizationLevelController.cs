using BLL.Project.SenderNumberOrganizationLevel;
using DTO.DataTable;
using DTO.Project.SenderNumberOrganizationLevel;
using Microsoft.AspNetCore.Mvc;

namespace PanelSMS.Areas.Project.SenderNumberOrganizationLevel.Controllers
{
    [Area("Project")]
    public class SenderNumberOrganizationLevelController : Controller
    {
        private readonly ISenderNumberOrganizationLevelManager _manager;

        public SenderNumberOrganizationLevelController(ISenderNumberOrganizationLevelManager manager)
        {
            _manager = manager;
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

            var result = _manager.GetDataTableDTO(search);
            return Json(result);
        }

        public IActionResult LoadCreateForm()
        {
            var model = new SenderNumberOrganizationLevelCreateDTO();
            return PartialView("_Create", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(SenderNumberOrganizationLevelCreateDTO model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new
                {
                    status = false,
                    message = "اطلاعات ارسال‌شده معتبر نیست!"
                });
            }

            var res = _manager.Create(model);
            return Json(res);
        }

        public IActionResult LoadEditForm(string id)
        {
            var model = _manager.GetById(id);

            if (model == null)
                return Content("خطا در دریافت اطلاعات.");

            return PartialView("_Edit", model);
        }

        [HttpPut]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(SenderNumberOrganizationLevelEditDTO model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new
                {
                    status = false,
                    message = "اطلاعات ارسال‌شده معتبر نیست!"
                });
            }

            var res = _manager.Update(model);
            return Json(res);
        }

        [HttpPost]
        public IActionResult Delete(string id)
        {
            var res = _manager.Delete(id);
            return Json(res);
        }
    }
}
