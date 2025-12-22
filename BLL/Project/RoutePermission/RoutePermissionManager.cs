using BLL.Interface;
using DTO.Base;
using DTO.Project.RoutePermission;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BLL
{
    public class RoutePermissionManager : IRoutePermissionManager
    {
        private readonly IHttpContextAccessor _httpContext;
        private readonly HttpClient _httpClient;
        private readonly ILogger<RoutePermissionManager> _logger;
        private readonly string _baseUrl;
        private readonly JsonSerializerOptions _jsonOptions;

        public RoutePermissionManager(
            IHttpContextAccessor httpContext,
            IConfiguration configuration,
            ILogger<RoutePermissionManager> logger,
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

        private async Task<T> ExecuteApiCallAsync<T>(Func<Task<HttpResponseMessage>> apiCall, string operationName)
        {
            try
            {
                await SetAuthorizationHeaderAsync();
                var response = await apiCall();

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    if (!string.IsNullOrWhiteSpace(content))
                    {
                        return JsonSerializer.Deserialize<T>(content, _jsonOptions);
                    }
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    // برای مواردی که یافت نمی‌شود
                    return default;
                }

                _logger.LogWarning("Operation {Operation} failed with status: {StatusCode}",
                    operationName, response.StatusCode);
                return default;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in {Operation}", operationName);
                return default;
            }
        }

        public async Task<List<RoutePermissionListDTO>> GetAllAsync()
        {
            var url = $"{_baseUrl}/api/admin/route-definitions";

            var response = await ExecuteApiCallAsync<ApiPermissionResponseListDTO<RoutePermissionListDTO>>(async () =>
            {
                return await _httpClient.GetAsync(url);
            }, nameof(GetAllAsync));

            return response?.Data ?? new List<RoutePermissionListDTO>();
        }

        public async Task<RoutePermissionEditDTO> GetByRouteKeyAsync(string routeKey)
        {
            if (string.IsNullOrWhiteSpace(routeKey))
                return null;

            // API مستقیم برای گرفتن تک آیتم ندارد، بنابراین از لیست فیلتر می‌کنیم
            var allPermissions = await GetAllAsync();
            var permission = allPermissions.FirstOrDefault(p => p.RouteKey == routeKey);

            if (permission == null)
                return null;

            return new RoutePermissionEditDTO
            {
                RouteKey = permission.RouteKey,
                Actions = permission.Actions ?? new List<string>(),
                RequiresFreshAuth = permission.RequiresFreshAuth
            };
        }

        public async Task<BaseResult> UpdateAsync(RoutePermissionEditDTO model)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(model.RouteKey))
                    return new BaseResult { Status = false, Message = "Route Key الزامی است." };

                var url = $"{_baseUrl}/api/admin/route-definitions/{Uri.EscapeDataString(model.RouteKey)}";

                var requestBody = new
                {
                    actions = model.Actions ?? new List<string>(),
                    requiresFreshAuth = model.RequiresFreshAuth
                };

                await SetAuthorizationHeaderAsync();

                var json = JsonSerializer.Serialize(requestBody, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync(url, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return new BaseResult
                    {
                        Status = true,
                        Message = "مجوز با موفقیت ویرایش شد."
                    };
                }

                // تلاش برای خواندن پیام خطا
                try
                {
                    var errorResponse = JsonSerializer.Deserialize<ApiErrorResponse>(responseContent, _jsonOptions);
                    return new BaseResult
                    {
                        Status = false,
                        Message = errorResponse?.Message ?? "خطا در ویرایش مجوز."
                    };
                }
                catch
                {
                    return new BaseResult
                    {
                        Status = false,
                        Message = responseContent ?? "خطا در ویرایش مجوز."
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in UpdateAsync");
                return new BaseResult
                {
                    Status = false,
                    Message = "خطا در ارتباط با سرور."
                };
            }
        }

        public async Task<BaseResult> DeleteAsync(string routeKey)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(routeKey))
                    return new BaseResult { Status = false, Message = "Route Key الزامی است." };

                var url = $"{_baseUrl}/api/admin/route-definitions/{Uri.EscapeDataString(routeKey)}";

                await SetAuthorizationHeaderAsync();
                var response = await _httpClient.DeleteAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    return new BaseResult
                    {
                        Status = true,
                        Message = "مجوز با موفقیت حذف شد."
                    };
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                return new BaseResult
                {
                    Status = false,
                    Message = responseContent ?? "خطا در حذف مجوز."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in DeleteAsync");
                return new BaseResult
                {
                    Status = false,
                    Message = "خطا در ارتباط با سرور."
                };
            }
        }

        public async Task<List<string>> GetSystemActionsAsync()
        {
            // لیست پیش‌فرض Actionهای سیستم
            // در واقعیت باید از API یا دیتابیس خوانده شود
            return new List<string>
            {
                "GET",
                "POST",
                "PUT",
                "DELETE",
                "PATCH",
                "HEAD",
                "OPTIONS",
                "VIEW",
                "CREATE",
                "EDIT",
                "DELETE",
                "EXPORT",
                "IMPORT",
                "APPROVE",
                "REJECT",
                "ASSIGN",
                "UNASSIGN",
                "ACTIVATE",
                "DEACTIVATE",
                "AUDIT"
            };
        }

        public async Task<List<RoutePermissionListDTO>> SearchAsync(string searchTerm)
        {
            var allPermissions = await GetAllAsync();

            if (string.IsNullOrWhiteSpace(searchTerm))
                return allPermissions;

            var normalizedSearch = searchTerm.Trim().ToLower();

            return allPermissions
                .Where(p =>
                    (p.RouteKey?.ToLower().Contains(normalizedSearch) ?? false) ||
                    (p.Actions?.Any(a => a.ToLower().Contains(normalizedSearch)) ?? false))
                .ToList();
        }
    }

    // DTO های کمکی
    internal class ApiPermissionResponseListDTO<T>
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
