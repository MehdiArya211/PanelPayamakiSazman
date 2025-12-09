using Microsoft.AspNetCore.Mvc;

namespace Software.Areas.Project.SenderNumberOrganizationLevel.Controllers
{
    [Area("Project")]
    public class SenderNumberOrganizationLevelController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
