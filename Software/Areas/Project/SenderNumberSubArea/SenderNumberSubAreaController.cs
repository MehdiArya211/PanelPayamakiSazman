using BLL.Project.SenderNumberSubArea;
using DTO.DataTable;
using DTO.Project.SenderNumberSubAreaList;
using Microsoft.AspNetCore.Mvc;

namespace Software.Areas.Project.SenderNumberSubArea
{
    [Area("Project")]
    public class SenderNumberSubAreaController : Controller
    {
        private readonly ISenderNumberSubAreaManager _senderNumberSubAreaManager;

        public SenderNumberSubAreaController(ISenderNumberSubAreaManager senderNumberSubAreaManager)
        {
            _senderNumberSubAreaManager = senderNumberSubAreaManager;
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

            var result = _senderNumberSubAreaManager.GetDataTableDTO(search);

            return Json(result);
        }

        public IActionResult LoadCreateForm()
        {
            var model = new SenderNumberSubAreaCreateDTO();
            return PartialView("_Create", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(SenderNumberSubAreaCreateDTO model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new
                {
                    status = false,
                    message = "اطلاعات ارسال‌شده معتبر نیست!"
                });
            }

            var res = _senderNumberSubAreaManager.Create(model);
            return Json(res);
        }


        public IActionResult LoadEditForm(string id)
        {
            var model = _senderNumberSubAreaManager.GetById(id);

            if (model == null)
                return Content("خطا در دریافت اطلاعات.");

            return PartialView("_Edit", model);
        }

        [HttpPut]
        [ValidateAntiForgeryToken]

        public IActionResult Edit(SenderNumberSubAreaEditDTO model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new
                {
                    status = false,
                    message = "اطلاعات ارسال‌شده معتبر نیست!"
                });
            }
            var res = _senderNumberSubAreaManager.Update(model);
            return Json(res);
        }


        [HttpPost]
        public IActionResult Delete(string id)
        {
            if (!ModelState.IsValid)
            {
                return Json(new
                {
                    status = false,
                    message = "اطلاعات ارسال‌شده معتبر نیست!"
                });
            }
            var result = _senderNumberSubAreaManager.Delete(id);
            return Json(result);
        }



    }
}
