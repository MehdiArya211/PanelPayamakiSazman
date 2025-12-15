using BLL;
using DTO;
using Microsoft.AspNetCore.Mvc;
using Services.SessionServices;

namespace PanelSMS.Areas.Project.Controllers
{
    [Area("Project")]
    public class UnitsController : Controller
    {
        private readonly IUnitsManager _UnitsManager;
        private readonly ISession session;

        public UnitsController(IUnitsManager unitsManager, IHttpContextAccessor _httpContextAccessor)
        {
            _UnitsManager = unitsManager;
            session = _httpContextAccessor.HttpContext.Session;

        }



        public async Task<IActionResult> Index()
        {
            var User = HttpContext.Session.GetUser();
            var request = new UnitSearchRequest
            {
                Page = 1,
                PageSize = 20
            };

            List<UnitsDTO> units = await _UnitsManager.SearchUnitsAsync(request, User);
            return View();
        }
    }
}
