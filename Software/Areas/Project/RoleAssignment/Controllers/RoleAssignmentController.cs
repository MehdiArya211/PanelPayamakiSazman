using BLL.Project.RoleAssignment;
using DTO.Project.RoleAssignment;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace PanelSMS.Areas.Project.RoleAssignment
{
    [Area("Project")]
    public class RoleAssignmentController : Controller
    {
        private readonly IRoleAssignmentManager _manager;

        public RoleAssignmentController(IRoleAssignmentManager manager)
        {
            _manager = manager;
        }

        // ---------------- VIEWS ----------------
        public IActionResult AssignUser()
        {
            return View();
        }

        public IActionResult AssignUnit()
        {
            return View();
        }

        // ---------------- AJAX APIs (UI) ----------------

        [HttpGet]
        public IActionResult GetUserRoleIds(Guid userId)
        {
            var roleIds = _manager.GetUserRoleIds(userId);
            return Json(new { roleIds });
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public IActionResult AssignToUser([FromBody] AssignRoleToUserDTO model)
        {
            if (model == null || model.UserId == Guid.Empty || model.RoleId == Guid.Empty)
                return BadRequest("اطلاعات نامعتبر است.");

            var res = _manager.AssignToUser(model);
            return Json(res);
        }

        [HttpPut]
        [IgnoreAntiforgeryToken]
        public IActionResult ReplaceUserRoles([FromQuery] Guid userId, [FromBody] ReplaceUserRolesDTO model)
        {
            if (userId == Guid.Empty)
                return BadRequest("userId نامعتبر است.");

            var res = _manager.ReplaceUserRoles(userId, model?.RoleIds);
            return Json(res);
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public IActionResult AssignToUnit([FromQuery] Guid unitId, [FromBody] AssignRoleToUnitDTO model)
        {
            if (unitId == Guid.Empty || model == null || model.RoleId == Guid.Empty)
                return BadRequest("اطلاعات نامعتبر است.");

            var res = _manager.AssignToUnit(unitId, model.RoleId);
            return Json(res);
        }
    }
}
