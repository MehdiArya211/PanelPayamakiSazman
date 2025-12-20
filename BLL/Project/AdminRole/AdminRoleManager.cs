using DTO.Base;
using DTO.DataTable;
using DTO.Project.AdminRole;
using DTO.WebApi;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BLL.Project.AdminRole
{
    public class AdminRoleManager : IAdminRoleManager
    {
        private readonly IHttpContextAccessor _httpContext;
        private readonly HttpClient _httpClient;
        private readonly ILogger<AdminRoleManager> _logger;
        private readonly string _baseUrl;
        private readonly JsonSerializerOptions _jsonOptions;

        public AdminRoleManager(
            IHttpContextAccessor httpContext,
            IConfiguration configuration,
            ILogger<AdminRoleManager> logger,
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

        public async Task<List<AdminRoleListDTO>> GetAllAsync()
        {
            var url = $"{_baseUrl}/api/admin/roles/search";

            var requestBody = new
            {
                page = 1,
                pageSize = 1000,
                filters = new List<object>(),
                sortBy = "name",
                sortDescending = false
            };

            var response = await ExecuteApiCallAsync<ApiResponsePagedDTO<AdminRoleListDTO>>(async () =>
            {
                var json = JsonSerializer.Serialize(requestBody, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                return await _httpClient.PostAsync(url, content);
            }, nameof(GetAllAsync));

            return response?.Data ?? new List<AdminRoleListDTO>();
        }

        public async Task<DataTableResponseDTO<AdminRoleListDTO>> GetDataTableDTOAsync(DataTableSearchDTO search)
        {
            var url = $"{_baseUrl}/api/admin/roles/search";

            var filters = new List<object>();

            if (!string.IsNullOrWhiteSpace(search.searchValue))
            {
                filters.Add(new
                {
                    field = "name",
                    @operator = "Contains",
                    value = search.searchValue
                });
            }

            var requestBody = new
            {
                page = (search.start / search.length) + 1,
                pageSize = search.length,
                filters = filters,
                sortBy = string.IsNullOrWhiteSpace(search.sortColumnName) ? "createdAt" : search.sortColumnName,
                sortDescending = search.sortDirection == "desc"
            };

            try
            {
                await SetAuthorizationHeaderAsync();

                var json = JsonSerializer.Serialize(requestBody, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonSerializer.Deserialize<ApiResponsePagedDTO<AdminRoleListDTO>>(
                        responseContent, _jsonOptions);

                    return new DataTableResponseDTO<AdminRoleListDTO>
                    {
                        draw = search.draw,
                        recordsTotal = apiResponse?.Meta?.TotalCount ?? 0,
                        recordsFiltered = apiResponse?.Meta?.TotalCount ?? 0,
                        data = apiResponse?.Data ?? new List<AdminRoleListDTO>()
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetDataTableDTOAsync");
            }

            return new DataTableResponseDTO<AdminRoleListDTO>
            {
                draw = search.draw,
                recordsTotal = 0,
                recordsFiltered = 0,
                data = new List<AdminRoleListDTO>()
            };
        }

        public async Task<AdminRoleEditDTO> GetByIdAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return null;

            var url = $"{_baseUrl}/api/admin/roles/{id}";

            var response = await ExecuteApiCallAsync<ApiResponseSingleDTO<AdminRoleEditDTO>>(async () =>
            {
                return await _httpClient.GetAsync(url);
            }, nameof(GetByIdAsync));

            return response?.Data;
        }

        public async Task<BaseResult> CreateAsync(AdminRoleCreateDTO model)
        {
            try
            {
                var url = $"{_baseUrl}/api/admin/roles";

                var requestBody = new
                {
                    name = model.Name,
                    description = model.Description
                };

                await SetAuthorizationHeaderAsync();

                var json = JsonSerializer.Serialize(requestBody, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(url, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return new BaseResult
                    {
                        Status = true,
                        Message = "نقش با موفقیت ایجاد شد."
                    };
                }

                // تلاش برای خواندن پیام خطا
                try
                {
                    var errorResponse = JsonSerializer.Deserialize<ApiErrorResponse>(responseContent, _jsonOptions);
                    return new BaseResult
                    {
                        Status = false,
                        Message = errorResponse?.Message ?? "خطا در ایجاد نقش."
                    };
                }
                catch
                {
                    return new BaseResult
                    {
                        Status = false,
                        Message = responseContent ?? "خطا در ایجاد نقش."
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in CreateAsync");
                return new BaseResult
                {
                    Status = false,
                    Message = "خطا در ارتباط با سرور."
                };
            }
        }

        public async Task<BaseResult> UpdateAsync(AdminRoleEditDTO model)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(model.Id))
                    return new BaseResult { Status = false, Message = "شناسه نقش نامعتبر است." };

                var url = $"{_baseUrl}/api/admin/roles/{model.Id}";

                var requestBody = new
                {
                    name = model.Name,
                    description = model.Description,
                    isActive = model.IsActive
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
                        Message = "نقش با موفقیت ویرایش شد."
                    };
                }

                try
                {
                    var errorResponse = JsonSerializer.Deserialize<ApiErrorResponse>(responseContent, _jsonOptions);
                    return new BaseResult
                    {
                        Status = false,
                        Message = errorResponse?.Message ?? "خطا در ویرایش نقش."
                    };
                }
                catch
                {
                    return new BaseResult
                    {
                        Status = false,
                        Message = responseContent ?? "خطا در ویرایش نقش."
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

        public async Task<BaseResult> DeleteAsync(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                    return new BaseResult { Status = false, Message = "شناسه نقش نامعتبر است." };

                var url = $"{_baseUrl}/api/admin/roles/{id}";

                await SetAuthorizationHeaderAsync();
                var response = await _httpClient.DeleteAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    return new BaseResult
                    {
                        Status = true,
                        Message = "نقش با موفقیت حذف شد."
                    };
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                return new BaseResult
                {
                    Status = false,
                    Message = responseContent ?? "خطا در حذف نقش."
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

        public async Task<BaseResult> ToggleActiveAsync(string id, bool isActive)
        {
            try
            {
                // ابتدا نقش را دریافت می‌کنیم
                var role = await GetByIdAsync(id);
                if (role == null)
                    return new BaseResult { Status = false, Message = "نقش مورد نظر یافت نشد." };

                // سپس با وضعیت جدید آپدیت می‌کنیم
                role.IsActive = isActive;
                return await UpdateAsync(role);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ToggleActiveAsync");
                return new BaseResult
                {
                    Status = false,
                    Message = "خطا در تغییر وضعیت نقش."
                };
            }
        }

        public async Task<bool> IsNameUniqueAsync(string name, string excludeId = null)
        {
            try
            {
                // در صورت لزوم می‌توان API جداگانه برای چک یکتایی اضافه کرد
                // فعلاً با لیست گرفتن و چک کردن در حافظه انجام می‌دهیم
                var roles = await GetAllAsync();

                if (roles == null || !roles.Any())
                    return true;

                var normalizedName = name.Trim().ToLower();

                if (!string.IsNullOrWhiteSpace(excludeId))
                {
                    return !roles.Any(r =>
                        r.Id != excludeId &&
                        r.Name.Trim().ToLower() == normalizedName);
                }

                return !roles.Any(r => r.Name.Trim().ToLower() == normalizedName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in IsNameUniqueAsync");
                return false;
            }
        }

        public async Task<List<SelectListItem>> GetActiveRolesForDropdownAsync()
        {
            try
            {
                var roles = await GetAllAsync();
                var activeRoles = roles?.Where(r => r.IsActive).ToList() ?? new List<AdminRoleListDTO>();

                return activeRoles.Select(r => new SelectListItem
                {
                    Value = r.Id,
                    Text = r.Name
                }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetActiveRolesForDropdownAsync");
                return new List<SelectListItem>();
            }
        }
    }

    // DTO داخلی برای خطاها
    internal class ApiErrorResponse
    {
        public string Message { get; set; }
        public List<string> Errors { get; set; }
    }
}
