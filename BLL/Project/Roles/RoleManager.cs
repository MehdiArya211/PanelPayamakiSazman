using DTO.Base;
using DTO.DataTable;
using DTO.Project.Roles;
using DTO.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Project.Roles
{
    public class RoleManager : IRoleManager
    {
        private readonly HttpClient _httpClient;

        public RoleManager(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<RoleDTO>> GetAllRolesAsync(DataTableSearchDTO search)
        {
            var response = await _httpClient.PostAsJsonAsync("/api/admin/roles/search", search);
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadFromJsonAsync<ApiResponsePagedDTO<RoleDTO>>();
                return data?.Data ?? new List<RoleDTO>();
            }
            return new List<RoleDTO>();
        }

        public async Task<RoleDTO> GetRoleByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"/api/admin/roles/{id}");
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadFromJsonAsync<ApiResponseSingleDTO<RoleDTO>>();
                return data?.Data;
            }
            return null;
        }

        public async Task<BaseResult> CreateRoleAsync(CreateRoleDTO model)
        {
            var response = await _httpClient.PostAsJsonAsync("/api/admin/roles", model);
            return await response.Content.ReadFromJsonAsync<BaseResult>();
        }

        public async Task<BaseResult> UpdateRoleAsync(int id, UpdateRoleDTO model)
        {
            var response = await _httpClient.PutAsJsonAsync($"/api/admin/roles/{id}", model);
            return await response.Content.ReadFromJsonAsync<BaseResult>();
        }

        public async Task<BaseResult> DeleteRoleAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"/api/admin/roles/{id}");
            return await response.Content.ReadFromJsonAsync<BaseResult>();
        }
    }
}
