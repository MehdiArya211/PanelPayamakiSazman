using BLL.Project.SenderNumberAssignment;
using DTO.DataTable;
using DTO.Project.SenderNumberAssignment;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Software.Areas.Project.SenderNumberAssignment.Controllers
{
    [Area("Project")]
    public class SenderNumberAssignmentController : Controller
    {
        private readonly ISenderNumberAssignmentManager _assignmentManager;

        public SenderNumberAssignmentController(ISenderNumberAssignmentManager assignmentManager)
        {
            _assignmentManager = assignmentManager;
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

            var result = _assignmentManager.GetDataTableDTO(search);
            return Json(result);
        }

        /* ------------ Create ------------ */

        public IActionResult LoadCreateForm()
        {
            var senderLookup = _assignmentManager.GetSenderNumberLookup();
            var userLookup = _assignmentManager.GetUserLookup();
            var clientLookup = _assignmentManager.GetClientLookup();

            var vm = new SenderNumberAssignmentFormViewModel
            {
                CreateModel = new SenderNumberAssignmentCreateDTO(),

                SenderNumberOptions = senderLookup
                    .ConvertAll(x => new SelectListItem
                    {
                        Value = x.Id,
                        Text = x.Text
                    }),

                UserOptions = userLookup
                    .ConvertAll(x => new SelectListItem
                    {
                        Value = x.Id,
                        Text = x.Text
                    }),

                ClientOptions = clientLookup
                    .ConvertAll(x => new SelectListItem
                    {
                        Value = x.Id,
                        Text = x.Text
                    })
            };

            return PartialView("_Create", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(SenderNumberAssignmentCreateDTO model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new
                {
                    status = false,
                    message = "اطلاعات ارسال‌شده معتبر نیست!"
                });
            }

            var res = _assignmentManager.Create(model);
            return Json(res);
        }

        /* ------------ Edit (Description only) ------------ */

        public IActionResult LoadEditForm(string id)
        {
            var editModel = _assignmentManager.GetById(id);

            if (editModel == null)
                return Content("خطا در دریافت اطلاعات تخصیص.");

            return PartialView("_Edit", editModel);
        }

        [HttpPut]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(SenderNumberAssignmentEditDTO model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new
                {
                    status = false,
                    message = "اطلاعات ارسال‌شده معتبر نیست!"
                });
            }

            var res = _assignmentManager.Update(model);
            return Json(res);
        }

        /* ------------ Revoke ------------ */

        [HttpPut]
        public IActionResult Revoke(string id, string reason)
        {
            var res = _assignmentManager.Revoke(id, reason);
            return Json(res);
        }
    }
}
