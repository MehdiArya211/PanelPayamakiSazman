using BLL.Interface;
using BLL.Project.SystemRole;
using DTO.DataTable;
using DTO.Project.SystemRole;
using Microsoft.AspNetCore.Mvc;

namespace PanelSMS.Areas.Project.SystemRole.Controllers
{
    [Area("Project")]

    public class SystemRoleController : Controller
    {
        private readonly ISystemRoleManager _systemRoleManager;

        public SystemRoleController(ISystemRoleManager systemRoleManager)
        {
            _systemRoleManager = systemRoleManager;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult GetList()
        {
            var search = new DataTableSearchDTO();

            search.start = int.Parse(Request.Form["start"]);
            search.length = int.Parse(Request.Form["length"]);
            search.draw = Request.Form["draw"];

            var colIndex = Request.Form["order[0][column]"];
            search.sortDirection = Request.Form["order[0][dir]"];
            search.sortColumnName = Request.Form[$"columns[{colIndex}][data]"];

            return Json(_systemRoleManager.GetDataTableDTO(search));
        }

        public IActionResult LoadCreateForm()
        {
            return PartialView("_Create", new SystemRoleCreateDTO());
        }

        [HttpPost]
        public IActionResult Create(SystemRoleCreateDTO model)
        {
            return Json(_systemRoleManager.Create(model));
        }

        public IActionResult LoadEditForm(string id)
        {
            var model = _systemRoleManager.GetById(id);

            if (model == null)
                return Content("خطا در دریافت اطلاعات.");

            return PartialView("_Edit", model);
        }

        [HttpPut]
        public IActionResult Edit(SystemRoleEditDTO model)
        {
            return Json(_systemRoleManager.Update(model));
        }

        [HttpPost]
        public IActionResult Delete(string id)
        {
            return Json(_systemRoleManager.Delete(id));
        }

        // این همون چیزیه که JS می‌خواد
        [HttpGet]
        public IActionResult GetAllForSelect()
        
        {
            var search = new DataTableSearchDTO
            {
                start = 0,
                length = 1000,
                draw = "1",
                sortColumnName = "Name",
                sortDirection = "asc"
            };

            var result = _systemRoleManager.GetDataTableDTO(search);

            return Json(result.data);
        }
    }
}
