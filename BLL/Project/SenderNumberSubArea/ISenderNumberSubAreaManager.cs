using DTO.Base;
using DTO.DataTable;
using DTO.Project.SenderNumberSubAreaList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Project.SenderNumberSubArea
{
    public interface ISenderNumberSubAreaManager
    {
        /// <summary>
        /// گرفتن همه‌ لیست از API
        /// </summary>
        List<SenderNumberSubAreaListDTO> GetAll();

        /// <summary>
        /// ساخت زیربخش
        /// </summary>
        BaseResult Create(SenderNumberSubAreaCreateDTO model);

        /// <summary>
        /// داده مناسب DataTable
        /// </summary>
        DataTableResponseDTO<SenderNumberSubAreaListDTO> GetDataTableDTO(DataTableSearchDTO search);

        /// <summary>
        /// دریافت یک رکورد برای ویرایش
        /// </summary>
        SenderNumberSubAreaEditDTO GetByCode(string code);

        /// <summary>
        /// ویرایش زیربخش
        /// </summary>
        BaseResult Update(SenderNumberSubAreaEditDTO model);

        public SenderNumberSubAreaEditDTO GetById(string id);


        /// <summary>
        /// حذف زیربخش
        /// </summary>
        BaseResult Delete(string code);
    }
}
