using DTO.Base;
using DTO.DataTable;
using DTO.Project.SenderNumberSpeciality;
using DTO.Project.SenderNumberSubAreaList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Project.SenderNumberSpeciality
{
    public interface ISenderNumberSpecialityManager
    {
        public DataTableResponseDTO<SenderNumberSpecialityListDTO> GetDataTableDTO(DataTableSearchDTO search);
        public BaseResult Create(SenderNumberSpecialityCreateDTO model);

    }
}
