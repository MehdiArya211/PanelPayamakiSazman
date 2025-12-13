using BLL.Project.RouteDefinition;
using DTO.Project.RouteDefinition;
using Microsoft.AspNetCore.Mvc;

namespace PanelSMS.Areas.Project.RouteDefinition.Controllers
{
    [Area("Project")]

    public class RouteDefinitionController : Controller
    {
        private readonly IRouteDefinitionManager _routeDefinitionManager;
        public RouteDefinitionController(IRouteDefinitionManager routeDefinitionManager)
        {
            _routeDefinitionManager = routeDefinitionManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult GetList()
        {
            var list = _routeDefinitionManager.GetAll();

            return Json(new
            {
                draw = Request.Form["draw"],
                recordsTotal = list.Count,
                recordsFiltered = list.Count,
                data = list
            });
        }

        public IActionResult LoadEditForm(string routeKey)
        {
            var model = _routeDefinitionManager
                .GetAll()
                .FirstOrDefault(x => x.RouteKey == routeKey);

            if (model == null)
                return Content("خطا در دریافت اطلاعات");

            return PartialView("_Edit", model);
        }

        [HttpPut]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(string routeKey, RouteDefinitionUpdateDTO model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { status = false, message = "اطلاعات نامعتبر است" });
            }

            var res = _routeDefinitionManager.Update(routeKey, model);
            return Json(res);
        }

        [HttpPost]
        public IActionResult Delete(string routeKey)
        {
            var res = _routeDefinitionManager.Delete(routeKey);
            return Json(res);
        }
    }

}
