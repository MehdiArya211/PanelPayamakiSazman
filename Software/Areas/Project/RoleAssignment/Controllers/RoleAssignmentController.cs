using BLL.Project.RoleAssignment;
using BLL.Project.SystemRole;
using BLL.Project.User;
using DTO.Project.RoleAssignment;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Linq;

namespace PanelSMS.Areas.Project.RoleAssignment
{
    [Area("Project")]
    public class RoleAssignmentController : Controller
    {
        private readonly IRoleAssignmentManager _manager;
        private readonly ISystemUserManager _systemUserManager;
        private readonly ISystemRoleManager _systemRoleManager;

        public RoleAssignmentController(
            IRoleAssignmentManager manager,
            ISystemUserManager systemUserManager,
            ISystemRoleManager systemRoleManager)
        {
            _manager = manager;
            _systemUserManager = systemUserManager;
            _systemRoleManager = systemRoleManager;
        }

        /* =====================================================
         * VIEWS
         * ===================================================== */

        public IActionResult AssignUser()
        {
            // lookup کاربران
            var users = _systemUserManager.GetUserLookup();

            // lookup نقش‌ها
            var roles = _systemRoleManager.GetRoleLookup();

            var vm = new RoleAssignmentUserFormViewModel
            {
                CreateModel = new AssignRoleToUserDTO(),

                UserOptions = users
                    .Select(x => new SelectListItem
                    {
                        Value = x.Id,
                        Text = x.Text
                    })
                    .ToList(),

                RoleOptions = roles
                    .Select(x => new SelectListItem
                    {
                        Value = x.Id,
                        Text = x.Text
                    })
                    .ToList()
            };

            return View(vm);
        }

        public IActionResult AssignUnit()
        {
            // این ViewModel رو مشابه AssignUser بعداً می‌سازی
            return View();
        }

        /* =====================================================
         * AJAX APIs (OPERATIONS)
         * ===================================================== */

        [HttpGet]
        public IActionResult GetUserRoleIds(Guid userId)
        {
            if (userId == Guid.Empty)
                return Json(new { roleIds = Array.Empty<Guid>() });

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
        public IActionResult ReplaceUserRoles(
            [FromQuery] Guid userId,
            [FromBody] ReplaceUserRolesDTO model)
        {
            if (userId == Guid.Empty)
                return BadRequest("userId نامعتبر است.");

            var res = _manager.ReplaceUserRoles(userId, model?.RoleIds);
            return Json(res);
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public IActionResult AssignToUnit(
            [FromQuery] Guid unitId,
            [FromBody] AssignRoleToUnitDTO model)
        {
            if (unitId == Guid.Empty || model == null || model.RoleId == Guid.Empty)
                return BadRequest("اطلاعات نامعتبر است.");

            var res = _manager.AssignToUnit(unitId, model.RoleId);
            return Json(res);
        }
    }
}
