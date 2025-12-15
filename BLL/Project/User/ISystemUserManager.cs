using DTO.Base;
using DTO.DataTable;
using DTO.Project.User;
using DTO.Project.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Project.User
{
    public interface ISystemUserManager
    {
        /// <summary>
        /// جستجو (DataTable)
        /// </summary>
        DataTableResponseDTO<SystemUserListDTO> GetDataTable(DataTableSearchDTO search);

        /// <summary>
        /// دریافت همه کاربران
        /// </summary>
        public List<LookupItemDTO> GetUserLookup();

        /// <summary>
        /// ایجاد کاربر جدید
        /// </summary>
        BaseResult Create(SystemUserCreateDTO model);

        /// <summary>
        /// دریافت یک کاربر با شناسه
        /// </summary>
        SystemUserEditDTO GetById(string id);

        /// <summary>
        /// ویرایش کاربر
        /// </summary>
        BaseResult Update(SystemUserEditDTO model);

        /// <summary>
        /// حذف کاربر
        /// </summary>
        BaseResult Delete(string id);
    }
}
