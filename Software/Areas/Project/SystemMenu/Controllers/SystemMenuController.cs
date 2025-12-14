using BLL.Project.RouteDefinition;
using BLL.Project.SystemMenu;
using DTO.DataTable;
using DTO.Project.SenderNumberSubAreaList;
using DTO.Project.SystemMenu;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace PanelSMS.Areas.Project.SystemMenu.Controllers
{
    [Area("Project")]
    public class SystemMenuController : Controller
    {
        private readonly ISystemMenuManager _systemMenuManager;
        private readonly IRouteDefinitionManager _routeDefinitionManager;

        public SystemMenuController(ISystemMenuManager systemMenuManager,
            IRouteDefinitionManager routeDefinitionManager)
        {
            _systemMenuManager = systemMenuManager;
            _routeDefinitionManager = routeDefinitionManager;
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

            var col = Request.Form["order[0][column]"];
            search.sortDirection = Request.Form["order[0][dir]"];
            search.sortColumnName = Request.Form[$"columns[{col}][data]"];

            var result = _systemMenuManager.GetDataTable(search);

            return Json(result);
        }

        public IActionResult LoadCreateForm()
        {
            var model = new SystemMenuCreateDTO();
            var ParentMenuList = _systemMenuManager.GetParentMenusForDropdown();
            ViewBag.ParentMenus = ParentMenuList
                        .Select(x => new SelectListItem
                        {
                            Value = x.Value,
                            Text = x.Text
                        })
                            .ToList();

            var routeDefinitionList = _routeDefinitionManager.GetAll();
            ViewBag.routeDefinitions = routeDefinitionList
                .Select(x => new SelectListItem
                {
                    Value = x.RouteKey,
                    Text = x.RouteKey
                })
                    .ToList();
            return PartialView("_Create", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(SystemMenuCreateDTO model)
        {
            if (!ModelState.IsValid)
                return Json(new { status = false, message = "اطلاعات معتبر نیست!" });

            var res = _systemMenuManager.Create(model);
            return Json(res);
        }

        public IActionResult LoadEditForm(string id)
        {
            var model = _systemMenuManager.GetById(id);
            return PartialView("_Edit", model);
        }

        [HttpPut]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(SystemMenuEditDTO model)
        {
            if (!ModelState.IsValid)
                return Json(new { status = false, message = "اطلاعات معتبر نیست!" });

            var res = _systemMenuManager.Update(model);
            return Json(res);
        }

        [HttpPost]
        public IActionResult Delete(string id)
        {
            var res = _systemMenuManager.Delete(id);
            return Json(res);
        }
    }

}
