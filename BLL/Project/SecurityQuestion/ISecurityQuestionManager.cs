using DTO.Base;
using DTO.DataTable;
using DTO.Project.SecurityQuestion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Project.SecurityQuestion
{
    public interface ISecurityQuestionManager
    {
        /// <summary>
        /// دریافت همه سوالات امنیتی از API
        /// </summary>
        List<SecurityQuestionListDTO> GetAll();

        /// <summary>
        /// ایجاد سوال امنیتی جدید
        /// </summary>
        BaseResult Create(SecurityQuestionCreateDTO model);

        /// <summary>
        /// خروجی مناسب برای DataTable
        /// </summary>
        DataTableResponseDTO<SecurityQuestionListDTO> GetDataTableDTO(DataTableSearchDTO search);

        /// <summary>
        /// دریافت یک سوال برای ویرایش با شناسه
        /// </summary>
        SecurityQuestionEditDTO GetById(string id);

        /// <summary>
        /// ویرایش سوال امنیتی
        /// </summary>
        BaseResult Update(SecurityQuestionEditDTO model);

        /// <summary>
        /// حذف سوال امنیتی
        /// </summary>
        BaseResult Delete(string id);
    }
}
