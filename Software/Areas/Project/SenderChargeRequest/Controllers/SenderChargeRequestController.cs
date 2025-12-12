using BLL.Project.SenderChargeRequest;
using DTO.DataTable;
using Microsoft.AspNetCore.Mvc;

namespace Software.Areas.Project.SenderChargeRequest.Controllers
{
    [Area("Project")]
    public class SenderChargeRequestController : Controller
    {
        private readonly IChargeRequestManager _chargeRequestManager;

        public SenderChargeRequestController(IChargeRequestManager chargeRequestManager)
        {
            _chargeRequestManager = chargeRequestManager;
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

            var result = _chargeRequestManager.GetDataTableDTO(search);
            return Json(result);
        }

        [HttpPut]
        public IActionResult Approve(string id, string note)
        {
            var res = _chargeRequestManager.Approve(id, note);
            return Json(res);
        }

        [HttpPut]
        public IActionResult Reject(string id, string note)
        {
            var res = _chargeRequestManager.Reject(id, note);
            return Json(res);
        }
    }
}
