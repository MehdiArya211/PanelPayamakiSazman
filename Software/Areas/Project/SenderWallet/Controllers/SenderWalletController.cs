using BLL.Project.SenderWallet;
using DTO.DataTable;
using DTO.Project.SenderWallet;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Software.Areas.Project.SenderWallet.Controllers
{
    [Area("Project")]
    public class SenderWalletController : Controller
    {
        private readonly ISenderWalletManager _walletManager;

        public SenderWalletController(ISenderWalletManager walletManager)
        {
            _walletManager = walletManager;
        }

        public IActionResult Index()
        {
            var lookup = _walletManager.GetSenderNumberLookup();

            var model = new SenderWalletIndexViewModel
            {
                SenderNumberOptions = lookup
                    .Select(x => new SelectListItem
                    {
                        Value = x.Id,
                        Text = x.Text
                    })
                    .ToList()
            };

            return View(model);
        }

        [HttpGet]
        public IActionResult GetWallet(string senderNumberId)
        {
            if (string.IsNullOrWhiteSpace(senderNumberId))
                return Json(null);

            var wallet = _walletManager.GetWallet(senderNumberId);
            return Json(wallet);
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public IActionResult GetTransactions(string senderNumberId)
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

            var result = _walletManager.GetTransactions(senderNumberId, search);
            return Json(result);
        }

        /* ------------ Charge ------------ */

        public IActionResult LoadChargeForm(string senderNumberId, string senderNumberText)
        {
            var model = new SenderWalletChargeDTO
            {
                SenderNumberId = senderNumberId
            };

            ViewBag.SenderNumberText = senderNumberText;

            return PartialView("_Charge", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Charge(SenderWalletChargeDTO model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new
                {
                    status = false,
                    message = "اطلاعات ارسال‌شده معتبر نیست!"
                });
            }

            var res = _walletManager.Charge(model);
            return Json(res);
        }

        /* ------------ Transfer ------------ */

        public IActionResult LoadTransferForm(string fromSenderNumberId, string fromSenderNumberText)
        {
            var lookup = _walletManager.GetSenderNumberLookup();

            var vm = new SenderWalletTransferViewModel
            {
                FromSenderNumberText = fromSenderNumberText,
                TransferModel = new SenderWalletTransferDTO
                {
                    FromSenderNumberId = fromSenderNumberId
                },
                SenderNumberOptions = lookup
                    .Select(x => new SelectListItem
                    {
                        Value = x.Id,
                        Text = x.Text
                    })
                    .ToList()
            };

            return PartialView("_Transfer", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Transfer(SenderWalletTransferDTO model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new
                {
                    status = false,
                    message = "اطلاعات ارسال‌شده معتبر نیست!"
                });
            }

            var res = _walletManager.Transfer(model);
            return Json(res);
        }
    }
}
