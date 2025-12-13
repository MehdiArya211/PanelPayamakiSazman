using DTO.Base;
using DTO.DataTable;
using DTO.Project.SystemRole;
using DTO.Project.WebApi;

namespace BLL.Project.SystemRole
{
    public interface ISystemRoleManager
    {
        public DataTableResponseDTO<SystemRoleListDTO> GetDataTableDTO(DataTableSearchDTO search);
        public BaseResult Create(SystemRoleCreateDTO model);
        public SystemRoleEditDTO GetById(string id);
        public BaseResult Update(SystemRoleEditDTO model);
        public BaseResult Delete(string id);

        public List<LookupItemDTO> GetRoleLookup();



    }
}
