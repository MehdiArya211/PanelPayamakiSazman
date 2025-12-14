using BLL.Project.SmsTariff;
using DTO.DataTable;
using DTO.Project.SmsTariffs;
using Microsoft.AspNetCore.Mvc;

namespace PanelSMS.Areas.Project.SmsTariff.Controllers
{
    [Area("Project")]
    public class SmsTariffsController : Controller
    {
        private readonly ISmsTariffManager _manager;

        public SmsTariffsController(ISmsTariffManager smsTariffManager)
        {
            _manager = smsTariffManager;
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

            var result = _manager.GetDataTableDTO(search);

            return Json(result);
        }

        #endregion


        #region ایجاد

        public IActionResult LoadCreateForm()
        {
            return PartialView("_Create", new SmsTariffCreateDTO());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(SmsTariffCreateDTO model)
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

        #endregion


        #region ویرایش

        public IActionResult LoadEditForm(string id)
        {
            var model = _manager.GetById(id);
            return PartialView("_Edit", model);
        }

        [HttpPut]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(SmsTariffEditDTO model)
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

        #endregion


        #region حذف

        [HttpPost]
        public IActionResult Delete(string id)
        {
            var res = _manager.Delete(id);
            return Json(res);
        }

        #endregion
    }
}
