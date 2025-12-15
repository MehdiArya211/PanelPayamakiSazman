using BLL.Project.Unit;
using DTO.DataTable;
using DTO.Project.Unit;
using Microsoft.AspNetCore.Mvc;

namespace PanelSMS.Areas.Project.Unit.Controllers
{
    [Area("Project")]
    public class UnitController : Controller
    {
        private readonly IUnitManager _unitManager;

        public UnitController(IUnitManager unitManager)
        {
            _unitManager = unitManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public IActionResult GetList()
        {
            var search = new DataTableSearchDTO();

            search.start = int.Parse(Request.Form["start"]);
            search.length = int.Parse(Request.Form["length"]);
            search.draw = Request.Form["draw"];
            search.searchValue = Request.Form["search[value]"];

            var colIndex = Request.Form["order[0][column]"];
            search.sortDirection = Request.Form["order[0][dir]"];
            search.sortColumnName = Request.Form[$"columns[{colIndex}][data]"];

            var result = _unitManager.GetDataTableDTO(search);

            return Json(result);
        }

        public IActionResult LoadCreateForm()
        {
            var model = new UnitCreateDTO();
            return PartialView("_Create", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(UnitCreateDTO model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new
                {
                    status = false,
                    message = "اطلاعات ارسال‌شده معتبر نیست!"
                });
            }

            var res = _unitManager.Create(model);
            return Json(res);
        }

        public IActionResult LoadEditForm(string id)
        {
            var model = _unitManager.GetById(id);

            if (model == null)
                return Content("خطا در دریافت اطلاعات واحد.");

            return PartialView("_Edit", model);
        }

        [HttpPut]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(UnitEditDTO model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new
                {
                    status = false,
                    message = "اطلاعات ارسال‌شده معتبر نیست!"
                });
            }

            var res = _unitManager.Update(model);
            return Json(res);
        }
    }
}
