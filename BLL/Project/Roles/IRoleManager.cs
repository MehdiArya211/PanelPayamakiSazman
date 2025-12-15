using DTO.Base;
using DTO.DataTable;
using DTO.Project.Roles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Project.Roles
{
    public interface IRoleManager
    {
        Task<List<RoleDTO>> GetAllRolesAsync(DataTableSearchDTO search);
        Task<RoleDTO> GetRoleByIdAsync(int id);
        Task<BaseResult> CreateRoleAsync(CreateRoleDTO model);
        Task<BaseResult> UpdateRoleAsync(int id, UpdateRoleDTO model);
        Task<BaseResult> DeleteRoleAsync(int id);
    }
}
