using BLL.Project.SmsManager;
using DTO.DataTable;
using DTO.Project.SmsManager;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Software.Areas.Project.SmsManager.Controllers
{
    [Area("Project")]
    public class AdminSmsController : Controller
    {
        private readonly IAdminSmsManager _smsManager;

        public AdminSmsController(IAdminSmsManager smsManager)
        {
            _smsManager = smsManager;
        }

        public IActionResult Index()
        {
            var senderLookup = _smsManager.GetSenderNumberLookup();
            var model = new AdminSmsIndexViewModel
            {
                SenderNumberOptions = senderLookup
                    .Select(x => new SelectListItem
                    {
                        Value = x.Id,
                        Text = x.Text
                    })
                    .ToList(),

                StatusOptions = new[]
                {
                    new SelectListItem { Value = "", Text = "همه وضعیت‌ها" },
                    new SelectListItem { Value = "Pending", Text = "در انتظار" },
                    new SelectListItem { Value = "Sent", Text = "ارسال شده" },
                    new SelectListItem { Value = "Delivered", Text = "تحویل شده" },
                    new SelectListItem { Value = "Failed", Text = "ناموفق" }
                }.ToList(),

                ChannelOptions = new[]
                {
                    new SelectListItem { Value = "", Text = "همه کانال‌ها" },
                    new SelectListItem { Value = "Portal", Text = "پرتال" },
                    new SelectListItem { Value = "Api", Text = "API" }
                }.ToList()
            };

            return View(model);
        }

        /* ------------ History (DataTable) ------------ */

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public IActionResult GetHistory(AdminSmsHistoryFilterDTO filter)
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

            var result = _smsManager.GetHistory(filter, search);
            return Json(result);
        }

        /* ------------ Single ------------ */

        public IActionResult LoadSingleForm()
        {
            var senderLookup = _smsManager.GetSenderNumberLookup();

            ViewBag.SenderNumberOptions = senderLookup
                .Select(x => new SelectListItem
                {
                    Value = x.Id,
                    Text = x.Text
                })
                .ToList();

            var model = new SmsSingleSendDTO();
            return PartialView("_Single", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SendSingle(SmsSingleSendDTO model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new
                {
                    status = false,
                    message = "اطلاعات ارسال‌شده معتبر نیست!"
                });
            }

            var res = _smsManager.SendSingle(model);
            return Json(res);
        }

        /* ------------ Bulk ------------ */

        public IActionResult LoadBulkForm()
        {
            var senderLookup = _smsManager.GetSenderNumberLookup();

            ViewBag.SenderNumberOptions = senderLookup
                .Select(x => new SelectListItem
                {
                    Value = x.Id,
                    Text = x.Text
                })
                .ToList();

            var model = new SmsBulkSendDTO();
            return PartialView("_Bulk", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SendBulk([FromForm] string SenderNumberId, [FromForm] string NumbersText, [FromForm] string Text)
        {
            // NumbersText: هر شماره در یک خط
            var dto = new SmsBulkSendDTO
            {
                SenderNumberId = SenderNumberId,
                Text = Text
            };

            if (!string.IsNullOrWhiteSpace(NumbersText))
            {
                var lines = NumbersText
                    .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var line in lines)
                {
                    var trimmed = line.Trim();
                    if (!string.IsNullOrWhiteSpace(trimmed))
                        dto.ReceiverNumbers.Add(trimmed);
                }
            }

            if (string.IsNullOrWhiteSpace(dto.SenderNumberId) || dto.ReceiverNumbers.Count == 0 || string.IsNullOrWhiteSpace(dto.Text))
            {
                return Json(new
                {
                    status = false,
                    message = "سرشماره، متن و حداقل یک شماره الزامی است."
                });
            }

            var res = _smsManager.SendBulk(dto);
            return Json(res);
        }

        /* ------------ Groups ------------ */

        public IActionResult LoadGroupsForm()
        {
            var senderLookup = _smsManager.GetSenderNumberLookup();
            var groupsLookup = _smsManager.GetContactGroupLookup();

            ViewBag.SenderNumberOptions = senderLookup
                .Select(x => new SelectListItem
                {
                    Value = x.Id,
                    Text = x.Text
                })
                .ToList();

            ViewBag.ContactGroupOptions = groupsLookup
                .Select(x => new SelectListItem
                {
                    Value = x.Id,
                    Text = x.Text
                })
                .ToList();

            var model = new SmsGroupSendDTO();
            return PartialView("_Groups", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SendGroups(SmsGroupSendDTO model)
        {
            if (!ModelState.IsValid || model.ContactGroupIds == null || !model.ContactGroupIds.Any())
            {
                return Json(new
                {
                    status = false,
                    message = "سرشماره، متن و حداقل یک گروه مخاطب الزامی است."
                });
            }

            var res = _smsManager.SendToGroups(model);
            return Json(res);
        }

        /* ------------ File ------------ */

        public IActionResult LoadFileForm()
        {
            var senderLookup = _smsManager.GetSenderNumberLookup();

            ViewBag.SenderNumberOptions = senderLookup
                .Select(x => new SelectListItem
                {
                    Value = x.Id,
                    Text = x.Text
                })
                .ToList();

            var model = new SmsFileSendViewModel();
            return PartialView("_File", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SendFile(SmsFileSendViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new
                {
                    status = false,
                    message = "سرشماره، فایل و متن پیام الزامی است."
                });
            }

            var res = _smsManager.SendFromFile(model);
            return Json(res);
        }
    }
}
