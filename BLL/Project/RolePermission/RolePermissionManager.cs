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

                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                        NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString
                    };

                    var apiResponse = JsonSerializer.Deserialize<ApiResponseListDTO<PermissionDto>>(content, options);

                    var permissions = apiResponse?.Data ?? new List<PermissionDto>();
                    var actionItems = await GetAllSystemActionsAsync();

                    return new RolePermissionViewModel
                    {
                        RoleId = roleId,
                        RoleName = roleName,
                        Permissions = permissions.OrderBy(p => p.PersianRouteName).ToList(), // مرتب بر اساس نام فارسی
                        ActionItems = actionItems
                    };
                }

                _logger.LogWarning("Failed to get permissions for role {RoleName}. Status: {StatusCode}",
                    roleName, response.StatusCode);
                return new RolePermissionViewModel { RoleId = roleId, RoleName = roleName };
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "JSON deserialization error in GetRolePermissionsAsync for role {RoleName}", roleName);
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

        public async Task<List<ActionItem>> GetAllSystemActionsAsync()
        {
            // لیست Actionها با نام فارسی
            return new List<ActionItem>
            {
                new ActionItem { Code = "GET", PersianName = "مشاهده" },
                new ActionItem { Code = "POST", PersianName = "ایجاد" },
                new ActionItem { Code = "PUT", PersianName = "ویرایش" },
                new ActionItem { Code = "DELETE", PersianName = "حذف" },
                new ActionItem { Code = "PATCH", PersianName = "به‌روزرسانی جزئی" },

                new ActionItem { Code = "VIEW", PersianName = "نمایش" },
                new ActionItem { Code = "CREATE", PersianName = "ساخت" },
                new ActionItem { Code = "EDIT", PersianName = "ویرایش" },
                new ActionItem { Code = "EXPORT", PersianName = "خروجی گرفتن" },
                new ActionItem { Code = "IMPORT", PersianName = "ورود اطلاعات" },

                new ActionItem { Code = "APPROVE", PersianName = "تایید" },
                new ActionItem { Code = "REJECT", PersianName = "رد" },
                new ActionItem { Code = "ASSIGN", PersianName = "اختصاص" },
                new ActionItem { Code = "UNASSIGN", PersianName = "لغو اختصاص" },

                new ActionItem { Code = "ACTIVATE", PersianName = "فعال‌سازی" },
                new ActionItem { Code = "DEACTIVATE", PersianName = "غیرفعال‌سازی" },
                new ActionItem { Code = "AUDIT", PersianName = "بازرسی" },
                new ActionItem { Code = "CONFIGURE", PersianName = "پیکربندی" },

                new ActionItem { Code = "DOWNLOAD", PersianName = "دانلود" },
                new ActionItem { Code = "UPLOAD", PersianName = "آپلود" },
                new ActionItem { Code = "PRINT", PersianName = "چاپ" },
                new ActionItem { Code = "SHARE", PersianName = "اشتراک‌گذاری" },

                new ActionItem { Code = "ENABLE", PersianName = "فعال کردن" },
                new ActionItem { Code = "DISABLE", PersianName = "غیرفعال کردن" },
                new ActionItem { Code = "RESET", PersianName = "بازنشانی" },
                new ActionItem { Code = "BACKUP", PersianName = "پشتیبان‌گیری" },

                new ActionItem { Code = "SEND", PersianName = "ارسال" },
                new ActionItem { Code = "RECEIVE", PersianName = "دریافت" },
                new ActionItem { Code = "VALIDATE", PersianName = "اعتبارسنجی" },
                new ActionItem { Code = "VERIFY", PersianName = "تایید اعتبار" }
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



        #region New
        private void SetAuth()
        {
            var token = _httpContext.HttpContext.Session.GetString("AdminToken");

            _httpClient.DefaultRequestHeaders.Authorization = null;

            if (!string.IsNullOrWhiteSpace(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }
        }

        private JsonSerializerOptions JsonOptions =>
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        public RolePermissionFormViewModel GetRolePermissions(string roleName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(roleName))
                    return null;

                SetAuth();

                var url = $"{_baseUrl}/api/admin/roles/{roleName}/permissions";

                var res = _httpClient.GetAsync(url).Result;
                if (!res.IsSuccessStatusCode)
                    return null;

                var jsonResult = res.Content.ReadAsStringAsync().Result;
                if (string.IsNullOrWhiteSpace(jsonResult))
                    return null;

                var apiResponse =
                    JsonSerializer.Deserialize<ApiResponsePagedDTO<PermissionDTO>>(
                        jsonResult, JsonOptions);

                var permissions = apiResponse?.Data ?? new List<PermissionDTO>();

                return new RolePermissionFormViewModel
                {
                    RoleName = roleName,
                    Permissions = permissions
                };
            }
            catch
            {
                return null;
            }
        }

        public BaseResult SaveRolePermissions(string roleName, List<RoutePermissionInputDTO> permissions)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(roleName) || permissions == null || permissions.Count == 0)
                    return new BaseResult
                    {
                        Status = false,
                        Message = "نام نقش یا مجوزهای نامعتبر هستند."
                    };

                SetAuth();

                var url = $"{_baseUrl}/api/admin/roles/{roleName}/permissions";

                var json = JsonSerializer.Serialize(permissions, JsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var res = _httpClient.PostAsync(url, content).Result;

                if (res.IsSuccessStatusCode)
                    return new BaseResult
                    {
                        Status = true,
                        Message = "مجوزهای نقش با موفقیت ذخیره شد."
                    };

                var msg = res.Content.ReadAsStringAsync().Result;
                return new BaseResult
                {
                    Status = false,
                    Message = string.IsNullOrWhiteSpace(msg) ? "خطا در ذخیره مجوزهای نقش." : msg
                };
            }
            catch (Exception ex)
            {
                return new BaseResult { Status = false, Message = ex.Message };
            }
        }
        #endregion


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
