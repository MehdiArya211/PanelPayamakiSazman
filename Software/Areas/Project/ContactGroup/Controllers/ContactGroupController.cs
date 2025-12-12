using BLL.Project.ContactGroup;
using DTO.DataTable;
using DTO.Project.ContactGroup;
using Microsoft.AspNetCore.Mvc;

namespace Software.Areas.Project.ContactGroup.Controllers
{
    [Area("Project")]
    public class ContactGroupController : Controller
    {
        private readonly IContactGroupManager _manager;

        public ContactGroupController(IContactGroupManager manager)
        {
            _manager = manager;
        }

        public IActionResult Index()
        {
            return View();
        }

        /* ------------------------------
         *  GROUPS DATATABLE
         * ------------------------------*/
        [HttpPost]
        [IgnoreAntiforgeryToken]
        public IActionResult GetGroups()
        {
            var search = new DataTableSearchDTO();

            search.start = int.Parse(Request.Form["start"]);
            search.length = int.Parse(Request.Form["length"]);
            search.draw = Request.Form["draw"];
            search.searchValue = Request.Form["search[value]"];

            var colIndex = Request.Form["order[0][column]"];
            search.sortDirection = Request.Form["order[0][dir]"];
            search.sortColumnName = Request.Form[$"columns[{colIndex}][data]"];

            var result = _manager.GetGroupsDataTable(search);
            return Json(result);
        }

        /* ------------------------------
         *  CREATE GROUP
         * ------------------------------*/
        public IActionResult LoadCreateForm()
        {
            var model = new ContactGroupCreateDTO();
            return PartialView("_Create", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ContactGroupCreateDTO model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { status = false, message = "اطلاعات ارسال‌شده معتبر نیست!" });
            }

            var res = _manager.CreateGroup(model);
            return Json(res);
        }

        /* ------------------------------
         *  EDIT GROUP
         * ------------------------------*/
        public IActionResult LoadEditForm(string id)
        {
            var model = _manager.GetGroupById(id);

            if (model == null)
                return Content("خطا در دریافت اطلاعات گروه.");

            return PartialView("_Edit", model);
        }

        [HttpPut]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(ContactGroupEditDTO model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { status = false, message = "اطلاعات ارسال‌شده معتبر نیست!" });
            }

            var res = _manager.UpdateGroup(model);
            return Json(res);
        }

        /* ------------------------------
         *  DELETE GROUP
         * ------------------------------*/
        [HttpPost]
        public IActionResult Delete(string id)
        {
            var res = _manager.DeleteGroup(id);
            return Json(res);
        }

        /* ============================================================
         *                    CONTACTS (inside group)
         * ============================================================*/

        public IActionResult LoadContactsModal(string groupId, string groupName)
        {
            var vm = new ContactGroupContactsViewModel
            {
                GroupId = groupId,
                GroupName = groupName
            };

            return PartialView("_Contacts", vm);
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public IActionResult GetContacts(string groupId)
        {
            var search = new DataTableSearchDTO();

            search.start = int.Parse(Request.Form["start"]);
            search.length = int.Parse(Request.Form["length"]);
            search.draw = Request.Form["draw"];
            search.searchValue = Request.Form["search[value]"];

            var colIndex = Request.Form["order[0][column]"];
            search.sortDirection = Request.Form["order[0][dir]"];
            search.sortColumnName = Request.Form[$"columns[{colIndex}][data]"];

            var result = _manager.GetContactsDataTable(groupId, search);
            return Json(result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddContact(string groupId, ContactInputDTO model)
        {
            if (!ModelState.IsValid)
                return Json(new { status = false, message = "اطلاعات ارسال‌شده معتبر نیست!" });

            var res = _manager.AddSingleContact(groupId, model);
            return Json(res);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddContactsText(string groupId, string numbers)
        {
            if (string.IsNullOrWhiteSpace(numbers))
                return Json(new { status = false, message = "شماره‌ها الزامی است." });

            var res = _manager.AddContactsByText(groupId, numbers);
            return Json(res);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UploadCsv(string groupId, IFormFile file)
        {
            if (file == null || file.Length == 0)
                return Json(new { status = false, message = "فایل معتبر نیست." });

            var res = _manager.UploadContactsCsv(groupId, file);
            return Json(res);
        }

        [HttpPost]
        public IActionResult Deduplicate(string groupId)
        {
            var res = _manager.Deduplicate(groupId);
            return Json(res);
        }
    }
}
