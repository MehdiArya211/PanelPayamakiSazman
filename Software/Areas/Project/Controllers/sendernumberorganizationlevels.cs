using Microsoft.AspNetCore.Mvc;

namespace Software.Areas.Project.Controllers
{
    public class sendernumberorganizationlevels : Controller
    {

        public sendernumberorganizationlevels()
        {
                
        }




        #region نمایش 
        public IActionResult Index()
        {
            return View();
        }
        #endregion


    }
}
