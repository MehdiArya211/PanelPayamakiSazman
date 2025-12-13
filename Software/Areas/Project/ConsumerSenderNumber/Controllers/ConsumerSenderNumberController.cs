using BLL.Project.ConsumerSenderNumbers;
using DTO.DataTable;
using Microsoft.AspNetCore.Mvc;

namespace PanelSMS.Areas.Project.ConsumerSenderNumber.Controllers
{
    [Area("Project")]
    public class ConsumerSenderNumberController : Controller
    {
        private readonly IConsumerSenderNumberManager _manager;

        public ConsumerSenderNumberController(IConsumerSenderNumberManager manager)
        {
            _manager = manager;
        }

        public IActionResult Index() => View();

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
    }
}
