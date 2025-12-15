using DTO.Base;
using DTO.DataTable;
using DTO.Project.ServiceClients;
using DTO.Project.WebApi;
using DTO.WebApi;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace BLL.Project.ServiceClients
{
    public class ServiceClientsManager : IServiceClientsManager
    {
        private readonly IHttpContextAccessor _httpContext;
        private readonly HttpClient _client;
        private readonly string _baseUrl;

        public ServiceClientsManager(IHttpContextAccessor accessor, IConfiguration config)
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
            {
                _client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }
        }

        /* ---------------------------
         * DataTable
         * --------------------------- */
        public DataTableResponseDTO<ServiceClientListDTO> GetDataTable(DataTableSearchDTO search)
        {
            SetAuth();

            var url = $"{_baseUrl}/service-clients/search";

            var body = new
            {
                page = (search.start / search.length) + 1,
                pageSize = search.length,
                filters = new List<object>(),
                sortBy = search.sortColumnName,
                sortDescending = search.sortDirection == "desc"
            };

            var content = new StringContent(
                JsonSerializer.Serialize(body),
                Encoding.UTF8,
                "application/json");

            var res = _client.PostAsync(url, content).Result;
            var json = res.Content.ReadAsStringAsync().Result;

            if (!res.IsSuccessStatusCode)
            {
                return new DataTableResponseDTO<ServiceClientListDTO>
                {
                    draw = search.draw,
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = new  List<ServiceClientListDTO>()
                };
            }

            var api =
                JsonSerializer.Deserialize<ApiResponsePagedDTO<ServiceClientListDTO>>(
                    json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return new DataTableResponseDTO<ServiceClientListDTO>
            {
                draw = search.draw,
                recordsTotal = api.Meta.TotalCount,
                recordsFiltered = api.Meta.TotalCount,
                data = api.Data
            };
        }

        /* ---------------------------
         * Get By Id
         * --------------------------- */
        public ServiceClientDetailDTO GetById(string clientId)
        {
            SetAuth();

            var url = $"{_baseUrl}/service-clients/{clientId}";

            var res = _client.GetAsync(url).Result;
            if (!res.IsSuccessStatusCode)
                return null;

            var json = res.Content.ReadAsStringAsync().Result;

            var api =
                JsonSerializer.Deserialize<ApiResponseSingleDTO<ServiceClientDetailDTO>>(
                    json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return api?.Data;
        }

        /* ---------------------------
         * Create
         * --------------------------- */
        public BaseResult Create0(ServiceClientCreateDTO model)
        {
            try
            {
                SetAuth();

                var url = $"{_baseUrl}/service-clients";

                var body = new Dictionary<string, object>
                {
                    ["unitId"] = model.UnitId,
                    ["displayName"] = model.DisplayName,
                    ["description"] = model.Description,
                    ["roleIds"] = model.RoleIds
                };

                var content = new StringContent(
                    JsonSerializer.Serialize(body),
                    Encoding.UTF8,
                    "application/json");

                var res = _client.PostAsync(url, content).Result;

                if (res.IsSuccessStatusCode)
                    return new BaseResult(true, "سرویس‌گیرنده با موفقیت ایجاد شد.");

                return new BaseResult(false, res.Content.ReadAsStringAsync().Result);
            }
            catch (Exception ex)
            {
                return new BaseResult(false, ex.Message);
            }
        }

        public BaseResult Create(ServiceClientCreateDTO model)
        {
            try
            {
                SetAuth();

                var url = $"{_baseUrl}/service-clients";

                // بدنه دقیقاً مطابق انتظار API
                var body = new
                {
                    unitId = model.UnitId,               // string uuid
                    displayName = model.DisplayName,
                    description = string.IsNullOrWhiteSpace(model.Description)
                        ? null
                        : model.Description,

                    roleIds = model.RoleIds ?? new List<string>()  // حتماً array
                };

                var json = JsonSerializer.Serialize(body, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var res = _client.PostAsync(url, content).Result;
                var responseBody = res.Content.ReadAsStringAsync().Result;

                if (res.IsSuccessStatusCode)
                    return new BaseResult(true, "سرویس‌گیرنده با موفقیت ایجاد شد.");

                // 👇 نمایش دقیق خطای API
                return new BaseResult(false,
                    $"API Error ({(int)res.StatusCode}): {responseBody}");
            }
            catch (Exception ex)
            {
                return new BaseResult(false, $"Exception: {ex.Message}");
            }
        }


        /* ---------------------------
         * Rotate Secret
         * --------------------------- */
        public BaseResult RotateSecret(string clientId, out string newSecret)
        {
            newSecret = null;

            try
            {
                SetAuth();

                var url = $"{_baseUrl}/service-clients/{clientId}/rotate-secret";

                var res = _client.PostAsync(url, null).Result;

                if (!res.IsSuccessStatusCode)
                    return new BaseResult(false, res.Content.ReadAsStringAsync().Result);

                var json = res.Content.ReadAsStringAsync().Result;

                var api =
                    JsonSerializer.Deserialize<ApiResponseSingleDTO<RotateSecretResultDTO>>(
                        json,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                newSecret = api?.Data?.ClientSecret;

                return new BaseResult(true, "کلید جدید با موفقیت ایجاد شد.");
            }
            catch (Exception ex)
            {
                return new BaseResult(false, ex.Message);
            }
        }

        /* ---------------------------
         * Change Status
         * --------------------------- */
        public BaseResult ChangeStatus(string clientId, bool isActive)
        {
            try
            {
                SetAuth();

                var url = $"{_baseUrl}/service-clients/{clientId}/status";

                var body = new { isActive };

                var content = new StringContent(
                    JsonSerializer.Serialize(body),
                    Encoding.UTF8,
                    "application/json");

                var res = _client.PutAsync(url, content).Result;

                if (res.IsSuccessStatusCode)
                    return new BaseResult(true, "وضعیت سرویس‌گیرنده تغییر یافت.");

                return new BaseResult(false, res.Content.ReadAsStringAsync().Result);
            }
            catch (Exception ex)
            {
                return new BaseResult(false, ex.Message);
            }
        }
    }
}
