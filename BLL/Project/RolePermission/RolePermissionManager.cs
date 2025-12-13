using DTO.Base;
using DTO.Project.RolePermission;
using DTO.WebApi;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace BLL.Project.RolePermission
{
    public class RolePermissionManager : IRolePermissionManager
    {
        private readonly IHttpContextAccessor _httpContext;
        private readonly HttpClient _client;
        private readonly string _baseUrl;

        public RolePermissionManager(IHttpContextAccessor accessor, IConfiguration config)
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

        public List<RolePermissionDTO> GetByRole(string roleName)
        {
            try
            {
                SetAuth();

                var url = $"{_baseUrl}/roles/{roleName}/permissions";
                var res = _client.GetAsync(url).Result;

                if (!res.IsSuccessStatusCode)
                    return new List<RolePermissionDTO>();

                var json = res.Content.ReadAsStringAsync().Result;

                var apiResponse =
                    JsonSerializer.Deserialize<ApiResponseListDTO<List<RolePermissionDTO>>>(
                        json,
                        new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });

                return apiResponse?.Data ?? new List<RolePermissionDTO>();
            }
            catch
            {
                return new List<RolePermissionDTO>();
            }
        }

        public BaseResult Save(string roleName, List<RolePermissionDTO> permissions)
        {
            try
            {
                SetAuth();

                var url = $"{_baseUrl}/roles/{roleName}/permissions";

                // پاکسازی: routeKey خالی نفرستیم
                permissions = permissions ?? new List<RolePermissionDTO>();
                permissions.RemoveAll(x => string.IsNullOrWhiteSpace(x.RouteKey));

                var json = JsonSerializer.Serialize(permissions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var res = _client.PostAsync(url, content).Result;

                if (res.IsSuccessStatusCode)
                    return new BaseResult(true, "مجوزها با موفقیت ذخیره شدند.");

                return new BaseResult(false, res.Content.ReadAsStringAsync().Result);
            }
            catch (Exception ex)
            {
                return new BaseResult(false, ex.Message);
            }
        }
    }
}
