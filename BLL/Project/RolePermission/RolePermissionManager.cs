using DTO.Base;
using DTO.Project.RolePermission;
using DTO.WebApi;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace BLL.Project.RolePermission
{
    public class RolePermissionManager : IRolePermissionManager
    {
        private readonly IHttpContextAccessor _httpContext;
        private readonly HttpClient _httpClient;
        private readonly ILogger<RolePermissionManager> _logger;
        private readonly string _baseUrl;
        private readonly JsonSerializerOptions _jsonOptions;

        public RolePermissionManager(
            IHttpContextAccessor httpContext,
            IConfiguration configuration,
            ILogger<RolePermissionManager> logger,
            IHttpClientFactory httpClientFactory)
        {
            _httpContext = httpContext;
            _logger = logger;
            _baseUrl = /*configuration["ApiSettings:BaseUrl"] ??*/ "http://87.107.111.44:8010";
            _httpClient = httpClientFactory.CreateClient("AdminApi");
            _httpClient.Timeout = TimeSpan.FromSeconds(30);

            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        private async Task SetAuthorizationHeaderAsync()
        {
            var token = _httpContext.HttpContext?.Session.GetString("AdminToken");

            _httpClient.DefaultRequestHeaders.Authorization = null;

            if (!string.IsNullOrWhiteSpace(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }
        }

        public async Task<RolePermissionViewModel> GetRolePermissionsAsync(string roleId, string roleName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(roleName))
                    return new RolePermissionViewModel();

                var url = $"{_baseUrl}/api/admin/roles/{Uri.EscapeDataString(roleName)}/permissions";

                await SetAuthorizationHeaderAsync();
                var response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonSerializer.Deserialize<ApiResponseListDTO<PermissionDto>>(content, _jsonOptions);

                    var permissions = apiResponse?.Data ?? new List<PermissionDto>();
                    var allActions = await GetAllSystemActionsAsync();

                    return new RolePermissionViewModel
                    {
                        RoleId = roleId,
                        RoleName = roleName,
                        Permissions = permissions.OrderBy(p => p.RouteKey).ToList(),
                        AllActions = allActions
                    };
                }

                _logger.LogWarning("Failed to get permissions for role {RoleName}. Status: {StatusCode}",
                    roleName, response.StatusCode);
                return new RolePermissionViewModel { RoleId = roleId, RoleName = roleName };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetRolePermissionsAsync for role {RoleName}", roleName);
                return new RolePermissionViewModel { RoleId = roleId, RoleName = roleName };
            }
        }

        public async Task<BaseResult> SaveRolePermissionsAsync(RolePermissionBulkUpdateDto model)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(model.RoleName))
                    return new BaseResult { Status = false, Message = "نام نقش الزامی است." };

                var url = $"{_baseUrl}/api/admin/roles/{Uri.EscapeDataString(model.RoleName)}/permissions";

                await SetAuthorizationHeaderAsync();

                // فقط Routeهایی که Action دارند را ارسال می‌کنیم
                var permissionsToSend = model.Permissions?
                    .Where(p => p.Actions != null && p.Actions.Any())
                    .ToList() ?? new List<RolePermissionInputDto>();

                var json = JsonSerializer.Serialize(permissionsToSend, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(url, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return new BaseResult
                    {
                        Status = true,
                        Message = $"مجوزهای نقش '{model.RoleName}' با موفقیت ذخیره شد."
                    };
                }

                try
                {
                    var errorResponse = JsonSerializer.Deserialize<ApiErrorResponse>(responseContent, _jsonOptions);
                    return new BaseResult
                    {
                        Status = false,
                        Message = errorResponse?.Message ?? "خطا در ذخیره مجوزها."
                    };
                }
                catch
                {
                    return new BaseResult
                    {
                        Status = false,
                        Message = responseContent ?? "خطا در ذخیره مجوزها."
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in SaveRolePermissionsAsync for role {RoleName}", model.RoleName);
                return new BaseResult
                {
                    Status = false,
                    Message = "خطا در ارتباط با سرور."
                };
            }
        }

        public async Task<List<string>> GetAllSystemActionsAsync()
        {
            // در واقعیت باید از API خاصی دریافت شود
            // فعلاً لیست ثابتی برمی‌گردانیم
            return new List<string>
            {
                "GET", "POST", "PUT", "DELETE", "PATCH",
                "VIEW", "CREATE", "EDIT", "EXPORT", "IMPORT",
                "APPROVE", "REJECT", "ASSIGN", "UNASSIGN",
                "ACTIVATE", "DEACTIVATE", "AUDIT", "CONFIGURE"
            };
        }

        public async Task<BaseResult> CopyPermissionsAsync(string sourceRoleName, string targetRoleName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(sourceRoleName) || string.IsNullOrWhiteSpace(targetRoleName))
                    return new BaseResult { Status = false, Message = "نام نقش‌ها الزامی است." };

                // مجوزهای نقش مبدا را می‌گیریم
                var sourcePermissions = await GetRolePermissionsAsync(null, sourceRoleName);
                if (sourcePermissions.Permissions == null || !sourcePermissions.Permissions.Any())
                    return new BaseResult { Status = false, Message = "نقش مبدا مجوزی ندارد." };

                // تبدیل به مدل bulk update
                var bulkUpdateDto = new RolePermissionBulkUpdateDto
                {
                    RoleName = targetRoleName,
                    Permissions = sourcePermissions.Permissions.Select(p => new RolePermissionInputDto
                    {
                        RouteKey = p.RouteKey,
                        Actions = p.AssignedActions?.ToList() ?? new List<string>()
                    }).ToList()
                };

                // ذخیره در نقش مقصد
                return await SaveRolePermissionsAsync(bulkUpdateDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in CopyPermissionsAsync from {Source} to {Target}",
                    sourceRoleName, targetRoleName);
                return new BaseResult
                {
                    Status = false,
                    Message = "خطا در کپی مجوزها."
                };
            }
        }

        public async Task<BaseResult> ResetPermissionsAsync(string roleName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(roleName))
                    return new BaseResult { Status = false, Message = "نام نقش الزامی است." };

                var url = $"{_baseUrl}/api/admin/roles/{Uri.EscapeDataString(roleName)}/permissions";

                await SetAuthorizationHeaderAsync();

                // ارسال آرایه خالی برای ریست کردن
                var json = JsonSerializer.Serialize(new List<object>(), _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    return new BaseResult
                    {
                        Status = true,
                        Message = $"مجوزهای نقش '{roleName}' با موفقیت ریست شد."
                    };
                }

                return new BaseResult
                {
                    Status = false,
                    Message = "خطا در ریست کردن مجوزها."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ResetPermissionsAsync for role {RoleName}", roleName);
                return new BaseResult
                {
                    Status = false,
                    Message = "خطا در ارتباط با سرور."
                };
            }
        }
    }

    // DTO های داخلی
    internal class ApiResponseListDTO<T>
    {
        public List<T> Data { get; set; }
        public object Meta { get; set; }
        public string TraceId { get; set; }
    }

    internal class ApiErrorResponse
    {
        public string Message { get; set; }
        public List<string> Errors { get; set; }
    }
}
