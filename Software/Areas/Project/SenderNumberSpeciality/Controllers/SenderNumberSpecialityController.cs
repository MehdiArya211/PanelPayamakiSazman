using BLL.Project.SenderNumberSpeciality;
using BLL.Project.SenderNumberSubArea;
using DTO.DataTable;
using DTO.Project.SenderNumberSpeciality;
using DTO.Project.SenderNumberSubAreaList;
using Microsoft.AspNetCore.Mvc;

namespace Software.Areas.Project.SenderNumberSpeciality.Controllers
{
    [Area("Project")]
    public class SenderNumberSpecialityController : Controller
    {
        private readonly ISenderNumberSpecialityManager _senderNumberSpecialityManager;

        public SenderNumberSpecialityController(ISenderNumberSpecialityManager senderNumberSpecialityManager)
        {
            _senderNumberSpecialityManager = senderNumberSpecialityManager;
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

            var result = _senderNumberSpecialityManager.GetDataTableDTO(search);

            return Json(result);
        }

        public IActionResult LoadCreateForm()
        {
            var model = new SenderNumberSpecialityCreateDTO();
            return PartialView("_Create", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(SenderNumberSpecialityCreateDTO model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new
                {
                    status = false,
                    message = "اطلاعات ارسال‌شده معتبر نیست!"
                });
            }

            var res = _senderNumberSpecialityManager.Create(model);
            return Json(res);
        }
    }
}
