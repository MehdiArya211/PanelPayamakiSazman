using BLL.Project.ServiceClients;
using BLL.Project.SystemRole;
using BLL.Project.Unit;
using DTO.DataTable;
using DTO.Project.ServiceClients;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace PanelSMS.Areas.Project.ServiceClients.Controllers
{
    [Area("Project")]

    public class ServiceClientsController : Controller
    {
        private readonly IServiceClientsManager _manager;
        private readonly ISystemRoleManager _systemRoleManager;
        private readonly IUnitManager _unitManager;

        public ServiceClientsController(IServiceClientsManager manager , ISystemRoleManager systemRoleManager,
            IUnitManager unitManager)
        {
            _manager = manager;
            _systemRoleManager = systemRoleManager;
            _unitManager = unitManager;
        }

        #region List

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

            var result = _manager.GetDataTable(search);
            return Json(result);
        }

        #endregion

        #region Create

        public IActionResult LoadCreateForm()
        {
            var unitList = _unitManager.GetUnitsForDropdown();
            var roleList = _systemRoleManager.GetRoleLookup();

            ViewBag.Units = unitList
                .Select(x => new SelectListItem
                {
                    Value = x.Value,
                    Text = x.Text
                })
                .ToList();

            ViewBag.Roles = roleList
                .Select(x => new SelectListItem
                {
                    Value = x.Id,
                    Text = x.Text
                })
                .ToList();
            return PartialView("_Create", new ServiceClientCreateDTO());
        }

        [HttpPost]
        public IActionResult Create(ServiceClientCreateDTO model)
        {
            if (!ModelState.IsValid)
                return Json(new { status = false, message = "اطلاعات نامعتبر است" });

            return Json(_manager.Create(model));
        }

        #endregion

        #region Rotate Secret

        [HttpPost]
        public IActionResult RotateSecret(string clientId)
        {
            var res = _manager.RotateSecret(clientId, out var secret);

            return Json(new
            {
                status = res.Status,
                message = res.Message,
                secret
            });
        }

        #endregion

        #region Change Status

        [HttpPost]
        public IActionResult ChangeStatus(string clientId, bool isActive)
        {
            return Json(_manager.ChangeStatus(clientId, isActive));
        }

        #endregion

        #region Edit

        public IActionResult LoadEditForm(string clientId)
        {
            var unitList = _unitManager.GetUnitsForDropdown();
            var roleList = _systemRoleManager.GetRoleLookup();

            ViewBag.Units = unitList
                .Select(x => new SelectListItem
                {
                    Value = x.Value,
                    Text = x.Text
                })
                .ToList();

            ViewBag.Roles = roleList
                .Select(x => new SelectListItem
                {
                    Value = x.Id,
                    Text = x.Text
                })
                .ToList();

            var model = _manager.GetById(clientId);
            return PartialView("_Edit", model);
        }

        [HttpPut]
        public IActionResult UpdateRoles(string clientId, List<string> roleIds)
        {
            return Json(_systemRoleManager.UpdateRoles(clientId, roleIds));
        }

        #endregion
    }

}
