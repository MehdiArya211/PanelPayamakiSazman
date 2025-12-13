using BLL.Project.SenderChargeRequestConsumer;
using DTO.DataTable;
using DTO.Project.SenderChargeRequestConsumer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace PanelSMS.Areas.Project.SenderChargeRequestConsumer.Controllers
{
    [Area("Project")]
    public class SenderChargeRequestConsumerController : Controller
    {
        private readonly ISenderChargeRequestConsumerManager _manager;

        public SenderChargeRequestConsumerController(ISenderChargeRequestConsumerManager manager)
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
            var senderNumbers = _manager.GetSenderNumberOptions();

            var vm = new SenderChargeRequestFormViewModel
            {
                CreateModel = new SenderChargeRequestCreateDTO(),
                SenderNumberOptions = senderNumbers.Select(x => new SelectListItem
                {
                    Value = x.Id,
                    Text = x.Display
                }).ToList()
            };

            return PartialView("_Create", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(SenderChargeRequestFormViewModel model)
        {
            if (!ModelState.IsValid)
                return Json(new { status = false, message = "اطلاعات ارسال‌شده معتبر نیست!" });

            var res = _manager.Create(model.CreateModel);
            return Json(res);
        }
    }
}
