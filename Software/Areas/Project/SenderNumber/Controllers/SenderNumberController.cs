using BLL.Project.SenderNumber;
using BLL.Project.SenderNumberOrganizationLevel;
using BLL.Project.SenderNumberSpeciality;
using BLL.Project.SenderNumberSubArea;
using DTO.DataTable;
using DTO.Project.SenderNumber;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Software.Areas.Project.SenderNumber.Controllers
{
    [Area("Project")]
    public class SenderNumberController : Controller
    {
        private readonly ISenderNumberManager _senderNumberManager;

        public SenderNumberController(ISenderNumberManager senderNumberManager)
        {
            _senderNumberManager = senderNumberManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public IActionResult GetList()
        {
            var search = new DataTableSearchDTO
            {
                start = int.Parse(Request.Form["start"]),
                length = int.Parse(Request.Form["length"]),
                draw = Request.Form["draw"],
                searchValue = Request.Form["search[value]"]
            };

            var colIndex = Request.Form["order[0][column]"];
            search.sortDirection = Request.Form["order[0][dir]"];
            search.sortColumnName = Request.Form[$"columns[{colIndex}][data]"];

            var result = _senderNumberManager.GetDataTableDTO(search);
            return Json(result);
        }

        /* ------------ Create ------------ */

        public IActionResult LoadCreateForm()
        {
            var model = new SenderNumberFormViewModel
            {
                CreateModel = new SenderNumberCreateDTO(),

                SpecialtyOptions = _senderNumberManager.GetAllSpecialities()
                    .Select(x => new SelectListItem
                    {
                        Value = x.Id,
                        Text = $"{x.Code} - {x.Title}"
                    }).ToList(),

                OrganizationLevelOptions = _senderNumberManager.GetAllOrganizationLevels()
                    .Select(x => new SelectListItem
                    {
                        Value = x.Id,
                        Text = $"{x.Code} - {x.Title}"
                    }).ToList(),

                SubAreaOptions = _senderNumberManager.GetAllSubAreas()
                    .Select(x => new SelectListItem
                    {
                        Value = x.Id,
                        Text = $"{x.Code} - {x.Title}"
                    }).ToList(),

                StatusOptions = new[]
                {
                    new SelectListItem { Value = "Purchasing", Text = "در حال خرید" },
                    new SelectListItem { Value = "CompletingDocuments", Text = "تکمیل مدارک" },
                    new SelectListItem { Value = "ReviewingDocuments", Text = "بررسی مدارک" },
                    new SelectListItem { Value = "Active", Text = "فعال" },
                    new SelectListItem { Value = "Inactive", Text = "غیرفعال" }
                }.ToList()
            };

            return PartialView("_Create", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(SenderNumberFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new
                {
                    status = false,
                    message = "اطلاعات ارسال‌شده معتبر نیست!"
                });
            }

            var res = _senderNumberManager.Create(model.CreateModel);
            return Json(res);
        }

        /* ------------ Edit ------------ */

        public IActionResult LoadEditForm(string id)
        {
            var editModel = _senderNumberManager.GetById(id);

            if (editModel == null)
                return Content("خطا در دریافت اطلاعات سرشماره.");

            var vm = new SenderNumberFormViewModel
            {
                EditModel = editModel,

                SpecialtyOptions = _senderNumberManager.GetAllSpecialities()
                    .Select(x => new SelectListItem
                    {
                        Value = x.Id,
                        Text = $"{x.Code} - {x.Title}",
                        Selected = x.Id == editModel.SpecialtyId
                    }).ToList(),

                OrganizationLevelOptions = _senderNumberManager.GetAllOrganizationLevels()
                    .Select(x => new SelectListItem
                    {
                        Value = x.Id,
                        Text = $"{x.Code} - {x.Title}",
                        Selected = x.Id == editModel.OrganizationLevelId
                    }).ToList(),

                SubAreaOptions = _senderNumberManager.GetAllSubAreas()
                    .Select(x => new SelectListItem
                    {
                        Value = x.Id,
                        Text = $"{x.Code} - {x.Title}",
                        Selected = x.Id == editModel.SubAreaId
                    }).ToList(),

                StatusOptions = new[]
                {
                    new SelectListItem { Value = "Purchasing", Text = "در حال خرید", Selected = editModel.Status == "Purchasing" },
                    new SelectListItem { Value = "CompletingDocuments", Text = "تکمیل مدارک", Selected = editModel.Status == "CompletingDocuments" },
                    new SelectListItem { Value = "ReviewingDocuments", Text = "بررسی مدارک", Selected = editModel.Status == "ReviewingDocuments" },
                    new SelectListItem { Value = "Active", Text = "فعال", Selected = editModel.Status == "Active" },
                    new SelectListItem { Value = "Inactive", Text = "غیرفعال", Selected = editModel.Status == "Inactive" }
                }.ToList()
            };

            return PartialView("_Edit", vm);
        }

        [HttpPut]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(SenderNumberEditDTO model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new
                {
                    status = false,
                    message = "اطلاعات ارسال‌شده معتبر نیست!"
                });
            }

            var res = _senderNumberManager.Update(model);
            return Json(res);
        }

        [HttpPost]
        public IActionResult Delete(string id)
        {
            var res = _senderNumberManager.Delete(id);
            return Json(res);
        }


        [HttpPut]
        public IActionResult ChangeStatus(string id, string status)
        {
            if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(status))
            {
                return Json(new
                {
                    status = false,
                    message = "شناسه یا وضعیت ارسال‌شده نامعتبر است."
                });
            }

            var res = _senderNumberManager.UpdateStatus(id, status);
            return Json(res);
        }
    }
}
