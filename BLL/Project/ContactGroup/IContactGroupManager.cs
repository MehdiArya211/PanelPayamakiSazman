using DTO.Base;
using DTO.DataTable;
using DTO.Project.ContactGroup;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Project.ContactGroup
{
    public interface IContactGroupManager
    {
        /* ------------------------------
         * GROUPS
         * ------------------------------*/

        /// <summary>
        /// خروجی دیتاتیبل گروه‌ها (server-side)
        /// </summary>
        DataTableResponseDTO<ContactGroupListDTO> GetGroupsDataTable(DataTableSearchDTO search);

        /// <summary>
        /// گرفتن همه گروه‌ها (برای پیدا کردن آیتم، یا ساختن dropdown در آینده)
        /// </summary>
        List<ContactGroupListDTO> GetAllGroups();

        /// <summary>
        /// گرفتن گروه با Id (چون GET نداریم از روی لیست پیدا می‌کنیم)
        /// </summary>
        ContactGroupEditDTO GetGroupById(string id);

        BaseResult CreateGroup(ContactGroupCreateDTO model);
        BaseResult UpdateGroup(ContactGroupEditDTO model);
        BaseResult DeleteGroup(string id);

        /* ------------------------------
         * CONTACTS (inside group)
         * ------------------------------*/

        DataTableResponseDTO<ContactDto> GetContactsDataTable(string groupId, DataTableSearchDTO search);

        /// <summary>
        /// افزودن یک مخاطب (API خودش چندتایی می‌گیرد، ما تک‌مخاطب می‌فرستیم)
        /// </summary>
        BaseResult AddSingleContact(string groupId, ContactInputDTO contact);

        /// <summary>
        /// افزودن گروهی با متن "0912...,0935..." 
        /// </summary>
        BaseResult AddContactsByText(string groupId, string numbers);

        /// <summary>
        /// آپلود CSV
        /// </summary>
        BaseResult UploadContactsCsv(string groupId, IFormFile file);

        /// <summary>
        /// حذف تکراری‌ها
        /// </summary>
        BaseResult Deduplicate(string groupId);
    }
}
