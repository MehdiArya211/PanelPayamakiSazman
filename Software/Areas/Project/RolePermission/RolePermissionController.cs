using BLL.Project.RolePermission;
using DTO.Project.RolePermission;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace PanelSMS.Areas.Project.RolePermission
{
    [Area("Project")]
    public class RolePermissionController : Controller
    {
        private readonly IRolePermissionManager _manager;

        public RolePermissionController(IRolePermissionManager manager)
        {
            _manager = manager;
        }

        public IActionResult Index(string roleName)
        {
            ViewBag.RoleName = roleName;
            return View();
        }

        [HttpGet]
        public IActionResult GetPermissions(string roleName)
        {
            var res = _manager.GetByRole(roleName);
            return Json(res);
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public IActionResult Save(
    [FromQuery] string roleName,
    [FromBody] List<RolePermissionDTO> permissions)
        {
            if (string.IsNullOrWhiteSpace(roleName))
                return BadRequest("roleName is required");

            var res = _manager.Save(roleName, permissions ?? new List<RolePermissionDTO>());
            return Json(res);
        }

    }
}
