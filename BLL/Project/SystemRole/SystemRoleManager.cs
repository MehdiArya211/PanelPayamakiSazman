using DTO.Base;
using DTO.DataTable;
using DTO.Project.SystemRole;
using DTO.WebApi;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using static System.Net.WebRequestMethods;

namespace BLL.Project.SystemRole
{
    public class SystemRoleManager : ISystemRoleManager
    {
        private readonly IHttpContextAccessor _httpContext;
        private readonly HttpClient _client;
         private readonly string _baseUrl;


        public SystemRoleManager(IHttpContextAccessor accessor, IConfiguration config)
        {
            _httpContext = accessor;
            _client = new HttpClient();
            _baseUrl = config["ApiBaseUrl"];

        }

        private void SetAuth()
        {
            var token = _httpContext.HttpContext.Session.GetString("AdminToken");
            _client.DefaultRequestHeaders.Authorization = null;

            if (!string.IsNullOrWhiteSpace(token))
                _client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
        }

        /* ---------------------------
         * Get All
         * --------------------------- */
        public List<SystemRoleListDTO> GetAll()
        {
            SetAuth();

            var url = $"{_baseUrl}/roles";

            var res = _client.GetAsync(url).Result;
            var json = res.Content.ReadAsStringAsync().Result;

            return JsonSerializer.Deserialize<List<SystemRoleListDTO>>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                ?? new List<SystemRoleListDTO>();
        }

        /* ---------------------------
         * DataTable
         * --------------------------- */
        public DataTableResponseDTO<SystemRoleListDTO> GetDataTableDTO(DataTableSearchDTO search)
        {
            SetAuth();

            var url = $"{_baseUrl}/roles/search";

            var body = new
            {
                page = (search.start / search.length) + 1,
                pageSize = search.length,
                filters = new List<object>(),
                sortBy = search.sortColumnName,
                sortDescending = search.sortDirection == "desc"
            };

            var json = JsonSerializer.Serialize(body);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var res = _client.PostAsync(url, content).Result;
            var jsonResult = res.Content.ReadAsStringAsync().Result;

            if (!res.IsSuccessStatusCode)
            {
                return new DataTableResponseDTO<SystemRoleListDTO>
                {
                    draw = search.draw,
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = new List<SystemRoleListDTO>(),
                    AdditionalData = jsonResult
                };
            }

            var api = JsonSerializer.Deserialize<ApiResponsePagedDTO<SystemRoleListDTO>>(
                jsonResult,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );

            return new DataTableResponseDTO<SystemRoleListDTO>
            {
                draw = search.draw,
                recordsTotal = api.Meta.TotalCount,
                recordsFiltered = api.Meta.TotalCount,
                data = api.Data
            };
        }

        /* ---------------------------
         * Create
         * --------------------------- */
        public BaseResult Create(SystemRoleCreateDTO model)
        {
            try
            {
                SetAuth();

                var url = $"{_baseUrl}/roles";

                var body = new
                {
                    name = model.Name,
                    description = model.Description,
                    isActive = model.IsActive
                };

                var json = JsonSerializer.Serialize(body);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var res = _client.PostAsync(url, content).Result;

                if (res.IsSuccessStatusCode)
                    return new BaseResult(true, "نقش با موفقیت ایجاد شد.");

                return new BaseResult(false, res.Content.ReadAsStringAsync().Result);
            }
            catch (Exception ex)
            {
                return new BaseResult(false, ex.Message);
            }
        }

        /* ---------------------------
         * Get By Id
         * --------------------------- */
        public SystemRoleEditDTO GetById(string id)
        {
            SetAuth();

            var url = $"{_baseUrl}/roles/{id}";


            var res = _client.GetAsync(url).Result;

            if (!res.IsSuccessStatusCode)
                return null;

            var json = res.Content.ReadAsStringAsync().Result;

            var apiResponse = JsonSerializer.Deserialize<ApiResponseSingleDTO<SystemRoleEditDTO>>(
                json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return apiResponse?.Data;
        }

        /* ---------------------------
         * Update
         * --------------------------- */
        public BaseResult Update(SystemRoleEditDTO model)
        {
            try
            {
                SetAuth();

                var url = $"{_baseUrl}/roles/{model.Id}";


                var body = new
                {
                    name = model.Name,
                    description = model.Description,
                    isActive = model.IsActive
                };

                var json = JsonSerializer.Serialize(body);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var res = _client.PutAsync(url, content).Result;

                if (res.IsSuccessStatusCode)
                    return new BaseResult(true, "نقش با موفقیت ویرایش شد.");

                return new BaseResult(false, res.Content.ReadAsStringAsync().Result);
            }
            catch (Exception ex)
            {
                return new BaseResult(false, ex.Message);
            }
        }

        /* ---------------------------
         * Delete
         * --------------------------- */
        public BaseResult Delete(string id)
        {
            try
            {
                SetAuth();

                var url = $"{_baseUrl}/roles/{id}";


                var res = _client.DeleteAsync(url).Result;

                if (res.IsSuccessStatusCode)
                    return new BaseResult(true, "نقش حذف شد.");

                return new BaseResult(false, res.Content.ReadAsStringAsync().Result);
            }
            catch (Exception ex)
            {
                return new BaseResult(false, ex.Message);
            }
        }
    }
}
