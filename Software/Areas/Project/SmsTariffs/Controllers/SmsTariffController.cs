using Microsoft.AspNetCore.Mvc;

namespace PanelSMS.Areas.Project.SmsTariffs.Controllers
{
    public class SmsTariffController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
