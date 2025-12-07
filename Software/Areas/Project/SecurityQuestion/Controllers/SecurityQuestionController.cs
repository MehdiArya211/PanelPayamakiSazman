using BLL.Project.SecurityQuestion;
using DTO.DataTable;
using DTO.Project.SecurityQuestion;
using Microsoft.AspNetCore.Mvc;

namespace Software.Areas.Project.SecurityQuestion.Controllers
{
    [Area("Project")]
    public class SecurityQuestionController : Controller
    {
        private readonly ISecurityQuestionManager _securityQuestionManager;

        public SecurityQuestionController(ISecurityQuestionManager securityQuestionManager)
        {
            _securityQuestionManager = securityQuestionManager;
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

            var result = _securityQuestionManager.GetDataTableDTO(search);

            return Json(result);
        }

        public IActionResult LoadCreateForm()
        {
            var model = new SecurityQuestionCreateDTO();
            return PartialView("_Create", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(SecurityQuestionCreateDTO model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new
                {
                    status = false,
                    message = "اطلاعات ارسال‌شده معتبر نیست!"
                });
            }

            var res = _securityQuestionManager.Create(model);
            return Json(res);
        }

        public IActionResult LoadEditForm(string id)
        {
            var model = _securityQuestionManager.GetById(id);

            if (model == null)
                return Content("خطا در دریافت اطلاعات.");

            return PartialView("_Edit", model);
        }

        [HttpPut]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(SecurityQuestionEditDTO model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new
                {
                    status = false,
                    message = "اطلاعات ارسال‌شده معتبر نیست!"
                });
            }

            var res = _securityQuestionManager.Update(model);
            return Json(res);
        }

        [HttpPost]
        public IActionResult Delete(string id)
        {
            var result = _securityQuestionManager.Delete(id);
            return Json(result);
        }
    }
}
