using BLL.Project.LoginAttempt;
using DTO.DataTable;
using Microsoft.AspNetCore.Mvc;

namespace PanelSMS.Areas.Project.LoginAttempt.Controllers
{
    [Area("Project")]
    public class LoginAttemptController : Controller
    {
        private readonly ILoginAttemptManager _loginAttemptManager;

        public LoginAttemptController(ILoginAttemptManager loginAttemptManager)
        {
            _loginAttemptManager = loginAttemptManager;
        }

        #region Login Attempts

        // GET: /Project/LoginAttempt/LoginAttempts
        public IActionResult LoginAttempts()
        {
            return View();
        }

        // POST: /Project/LoginAttempt/GetLoginAttempts
        [HttpPost]
        public IActionResult GetLoginAttempts()
        {
            var search = BuildSearchModel();
            var result = _loginAttemptManager.GetLoginAttemptsDataTable(search);
            return Json(result);
        }

        #endregion

        #region Security Question Attempts

        // GET: /Project/LoginAttempt/SecurityQuestionAttempts
        public IActionResult SecurityQuestionAttempts()
        {
            return View();
        }

        // POST: /Project/LoginAttempt/GetSecurityQuestionAttempts
        [HttpPost]
        public IActionResult GetSecurityQuestionAttempts()
        {
            var search = BuildSearchModel();
            var result = _loginAttemptManager.GetSecurityQuestionAttemptsDataTable(search);
            return Json(result);
        }

        #endregion

        #region Helpers

        private DataTableSearchDTO BuildSearchModel()
        {
            var search = new DataTableSearchDTO();

            search.start = int.Parse(Request.Form["start"]);
            search.length = int.Parse(Request.Form["length"]);
            search.draw = Request.Form["draw"];

            var colIndex = Request.Form["order[0][column]"];
            search.sortDirection = Request.Form["order[0][dir]"];
            search.sortColumnName = Request.Form[$"columns[{colIndex}][data]"];

            return search;
        }

        #endregion
    }
}
