using BLL.Interface;
using BLL.Project.Menus;
using DTO.DataTable;
using DTO.Project.Menus;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Software.Areas.Project.Menu.Controllers
{
    [Area("Project")]
    public class MenuController : Controller
    {
        private readonly IAdminMenuManager _menuManager;

        public MenuController(IAdminMenuManager menuManager)
        {
            _menuManager = menuManager;
        }

        public IActionResult Index()
        {
            var tree = _menuManager.GetTree();
            return View(tree);
        }

        // حذف GetList - چون دیگه از DataTable استفاده نمی‌کنیم

        public IActionResult LoadCreateForm()
        {
            var allMenus = _menuManager.GetAll();
            var model = new MenuFormViewModel
            {
                CreateModel = new MenuCreateDTO(),
                ParentOptions = allMenus
                    .Select(x => new SelectListItem
                    {
                        Value = x.Id,
                        Text = x.Title
                    }).ToList(),
                ActiveOptions = new List<SelectListItem>
                {
                    new SelectListItem { Value = "true", Text = "فعال" },
                    new SelectListItem { Value = "false", Text = "غیرفعال" }
                }
            };

            model.ParentOptions.Insert(0, new SelectListItem { Value = "", Text = "--- منوی اصلی (بدون والد) ---" });

            return PartialView("_Create", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(MenuFormViewModel model)
        {
            if (!ModelState.IsValid)
                return Json(new { status = false, message = "اطلاعات ارسال‌شده معتبر نیست!" });

            var res = _menuManager.Create(model.CreateModel);
            return Json(res);
        }

        public IActionResult LoadEditForm(string id)
        {
            var editModel = _menuManager.GetById(id);
            if (editModel == null)
                return Content("خطا در دریافت اطلاعات منو.");

            var allMenus = _menuManager.GetAll()
                .Where(x => x.Id != id) // حذف خود منو از لیست والدها
                .ToList();

            var vm = new MenuFormViewModel
            {
                EditModel = editModel,
                ParentOptions = allMenus
                    .Select(x => new SelectListItem
                    {
                        Value = x.Id,
                        Text = x.Title,
                        Selected = x.Id == editModel.ParentId
                    }).ToList(),
                ActiveOptions = new List<SelectListItem>
                {
                    new SelectListItem { Value = "true", Text = "فعال", Selected = editModel.IsActive },
                    new SelectListItem { Value = "false", Text = "غیرفعال", Selected = !editModel.IsActive }
                }
            };

            vm.ParentOptions.Insert(0, new SelectListItem
            {
                Value = "",
                Text = "--- منوی اصلی (بدون والد) ---",
                Selected = string.IsNullOrEmpty(editModel.ParentId)
            });

            return PartialView("_Edit", vm);
        }

        [HttpPut]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(MenuFormViewModel model)
        {
            if (!ModelState.IsValid)
                return Json(new { status = false, message = "اطلاعات ارسال‌شده معتبر نیست!" });

            var res = _menuManager.Update(model.EditModel);
            return Json(res);
        }

        [HttpPost]
        public IActionResult Delete(string id)
        {
            var res = _menuManager.Delete(id);
            return Json(res);
        }

        [HttpGet]
        public IActionResult GetMenuTree()
        {
            var tree = _menuManager.GetTree();
            return Json(new { success = true, data = tree });
        }
    }
}
