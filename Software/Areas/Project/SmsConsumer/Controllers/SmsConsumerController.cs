using BLL.Project.SmsConsumer;
using DTO.DataTable;
using DTO.Project.SmsConsumer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Software.Areas.Project.SmsConsumer.Controllers
{
    [Area("Project")]
    public class SmsConsumerController : Controller
    {
        private readonly ISmsConsumerManager _sms;

        public SmsConsumerController(ISmsConsumerManager sms)
        {
            _sms = sms;
        }

        public IActionResult Index()
        {
            // صفحه اصلی: تاریخچه + فرم ارسال
            return View();
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public IActionResult GetHistory(bool onlyOutbound = true)
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

            var result = _sms.GetHistoryDataTableDTO(search, onlyOutbound);
            return Json(result);
        }

        /* ---------- Load Forms ---------- */

        public IActionResult LoadSendForm()
        {
            var vm = new SmsSendViewModel();

            vm.SenderNumberOptions = _sms.GetSenderNumberOptions()
                .Select(x => new SelectListItem { Value = x.Id, Text = x.Display })
                .ToList();

            vm.ContactGroupOptions = _sms.GetContactGroupOptions()
                .Select(x => new SelectListItem { Value = x.Id, Text = x.Display })
                .ToList();

            return PartialView("_Send", vm);
        }

        /* ---------- Send Actions ---------- */

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SendSingle(SmsSendSingleDTO model)
        {
            if (!ModelState.IsValid)
                return Json(new { status = false, message = "اطلاعات ارسال‌شده معتبر نیست!" });

            var res = _sms.SendSingle(model);
            return Json(res);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SendBulk(SmsSendBulkDTO model)
        {
            if (!ModelState.IsValid)
                return Json(new { status = false, message = "اطلاعات ارسال‌شده معتبر نیست!" });

            var numbers = (model.ReceiverNumbersText ?? "")
                .Split(new[] { '\n', '\r', ',', ';', ' ' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct()
                .ToList();

            if (numbers.Count == 0)
                return Json(new { status = false, message = "حداقل یک شماره گیرنده وارد کنید." });

            var res = _sms.SendBulk(model.SenderNumberId, numbers, model.Text);
            return Json(res);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SendGroups(SmsSendGroupsDTO model)
        {
            if (!ModelState.IsValid)
                return Json(new { status = false, message = "اطلاعات ارسال‌شده معتبر نیست!" });

            var res = _sms.SendGroups(model);
            return Json(res);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SendFile(SmsSendFileDTO model)
        {
            if (!ModelState.IsValid)
                return Json(new { status = false, message = "اطلاعات ارسال‌شده معتبر نیست!" });

            var res = _sms.SendFile(model);
            return Json(res);
        }
    }
}
