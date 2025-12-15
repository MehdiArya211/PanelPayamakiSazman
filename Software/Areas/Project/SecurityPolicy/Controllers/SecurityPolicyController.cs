using BLL.Project.SecurityPolicy;
using DTO.Project.SecurityPolicy;
using Microsoft.AspNetCore.Mvc;

namespace PanelSMS.Areas.Project.SecurityPolicy.Controllers
{
    [Area("Project")]
    public class SecurityPolicyController : Controller
    {
        private readonly ISecurityPolicyManager _manager;

        public SecurityPolicyController(ISecurityPolicyManager manager)     
        {
            _manager = manager;
        }

        public IActionResult Index()
        {
            var model = _manager.Get();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Save(SecurityPolicyDTO model)
        {
            if (!ModelState.IsValid)
                return Json(new { status = false, message = "اطلاعات نامعتبر است" });

            var res = _manager.Update(model);
            return Json(res);
        }
    }
}
