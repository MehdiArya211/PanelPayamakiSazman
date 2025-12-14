using DTO.Base;
using DTO.DataTable;
using DTO.Project.SmsTariffs;

namespace BLL.Project.SmsTariff
{
    public interface ISmsTariffManager
    {
        DataTableResponseDTO<SmsTariffListDTO> GetDataTableDTO(DataTableSearchDTO search);

        SmsTariffEditDTO GetById(string id);

        BaseResult Create(SmsTariffCreateDTO model);

        BaseResult Update(SmsTariffEditDTO model);

        BaseResult Delete(string id);
    }
}
