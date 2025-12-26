using DTO.Base;
using DTO.DataTable;
using DTO.Project.AdminUser;
using DTO.WebApi;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BLL.Project.AdminUser
{
    public class AdminUserManager : IAdminUserManager
    {
        private readonly IHttpContextAccessor _httpContext;
        private readonly HttpClient _client;
        private readonly string _baseUrl;

        public AdminUserManager(IHttpContextAccessor accessor, IConfiguration config)
        {
            _httpContext = accessor;
            _client = new HttpClient();
            _baseUrl = "http://87.107.111.44:8010";
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

        private JsonSerializerOptions JsonOptions =>
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        public List<AdminUserSummaryDTO> GetAll()
        {
            try
            {
                SetAuth();
                var url = $"{_baseUrl}/api/admin/users/search";

                var body = new
                {
                    page = 1,
                    pageSize = 1000,
                    filters = new List<object>(),
                    sortBy = "userName",
                    sortDescending = false
                };

                var json = JsonSerializer.Serialize(body, JsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var res = _client.PostAsync(url, content).Result;
                var jsonResult = res.Content.ReadAsStringAsync().Result;

                if (!res.IsSuccessStatusCode || string.IsNullOrWhiteSpace(jsonResult))
                    return new List<AdminUserSummaryDTO>();

                var apiResponse = JsonSerializer.Deserialize<ApiResponsePagedDTO<AdminUserSummaryDTO>>(jsonResult, JsonOptions);
                return apiResponse?.Data ?? new List<AdminUserSummaryDTO>();
            }
            catch
            {
                return new List<AdminUserSummaryDTO>();
            }
        }

        public DataTableResponseDTO<AdminUserSummaryDTO> GetDataTableDTO(DataTableSearchDTO search)
        {
            try
            {
                SetAuth();
                var url = $"{_baseUrl}/api/admin/users/search";

                var body = new
                {
                    page = (search.start / search.length) + 1,
                    pageSize = search.length,
                    filters = new List<object>(),
                    sortBy = string.IsNullOrWhiteSpace(search.sortColumnName) ? "userName" : search.sortColumnName,
                    sortDescending = search.sortDirection == "desc"
                };

                var json = JsonSerializer.Serialize(body, JsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var res = _client.PostAsync(url, content).Result;
                var jsonResult = res.Content.ReadAsStringAsync().Result;

                if (!res.IsSuccessStatusCode || string.IsNullOrWhiteSpace(jsonResult))
                {
                    return new DataTableResponseDTO<AdminUserSummaryDTO>
                    {
                        draw = search.draw,
                        recordsTotal = 0,
                        recordsFiltered = 0,
                        data = new List<AdminUserSummaryDTO>()
                    };
                }

                var apiResponse = JsonSerializer.Deserialize<ApiResponsePagedDTO<AdminUserSummaryDTO>>(jsonResult, JsonOptions);
                var total = apiResponse?.Meta?.TotalCount ?? 0;
                var data = apiResponse?.Data ?? new List<AdminUserSummaryDTO>();

                return new DataTableResponseDTO<AdminUserSummaryDTO>
                {
                    draw = search.draw,
                    recordsTotal = total,
                    recordsFiltered = total,
                    data = data
                };
            }
            catch (Exception ex)
            {
                return new DataTableResponseDTO<AdminUserSummaryDTO>
                {
                    draw = search.draw,
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = new List<AdminUserSummaryDTO>(),
                    AdditionalData = ex.Message
                };
            }
        }

        public AdminUserDetailDTO GetById(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                    return null;

                SetAuth();
                var url = $"{_baseUrl}/api/admin/users/{id}";

                var res = _client.GetAsync(url).Result;
                if (!res.IsSuccessStatusCode)
                    return null;

                var jsonResult = res.Content.ReadAsStringAsync().Result;
                if (string.IsNullOrWhiteSpace(jsonResult))
                    return null;

                var apiResponse = JsonSerializer.Deserialize<ApiResponseSingleDTO<AdminUserDetailDTO>>(jsonResult, JsonOptions);
                return apiResponse?.Data;
            }
            catch
            {
                return null;
            }
        }

        public BaseResult Create(AdminUserCreateDTO model)
        {
            try
            {
                SetAuth();
                var url = $"{_baseUrl}/api/admin/users";

                var body = new
                {
                    unitId = model.UnitId,
                    userName = model.UserName,
                    initialPassword = model.InitialPassword,
                    firstName = model.FirstName,
                    lastName = model.LastName,
                    nationalCode = model.NationalCode,
                    mobileNumber = model.MobileNumber,
                    roleIds = model.RoleIds,
                    securityQuestions = model.SecurityQuestions
                };

                var json = JsonSerializer.Serialize(body, JsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var res = _client.PostAsync(url, content).Result;

                if (res.IsSuccessStatusCode)
                    return new BaseResult { Status = true, Message = "کاربر مدیر با موفقیت ایجاد شد." };

                var msg = res.Content.ReadAsStringAsync().Result;
                return new BaseResult
                {
                    Status = false,
                    Message = string.IsNullOrWhiteSpace(msg) ? "خطا در ایجاد کاربر مدیر." : msg
                };
            }
            catch (Exception ex)
            {
                return new BaseResult { Status = false, Message = ex.Message };
            }
        }

        public BaseResult Update(AdminUserEditDTO model)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(model.Id))
                    return new BaseResult { Status = false, Message = "شناسه کاربر نامعتبر است." };

                SetAuth();
                var url = $"{_baseUrl}/api/admin/users/{model.Id}";

                var body = new
                {
                    unitId = model.UnitId,
                    firstName = model.FirstName,
                    lastName = model.LastName,
                    nationalCode = model.NationalCode,
                    mobileNumber = model.MobileNumber,
                    isActive = model.IsActive,
                    roleIds = model.RoleIds
                };

                var json = JsonSerializer.Serialize(body, JsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var res = _client.PutAsync(url, content).Result;

                if (res.IsSuccessStatusCode)
                    return new BaseResult { Status = true, Message = "کاربر مدیر با موفقیت ویرایش شد." };

                var msg = res.Content.ReadAsStringAsync().Result;
                return new BaseResult
                {
                    Status = false,
                    Message = string.IsNullOrWhiteSpace(msg) ? "خطا در ویرایش کاربر مدیر." : msg
                };
            }
            catch (Exception ex)
            {
                return new BaseResult { Status = false, Message = ex.Message };
            }
        }

        public BaseResult Delete(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                    return new BaseResult { Status = false, Message = "شناسه کاربر نامعتبر است." };

                SetAuth();
                var url = $"{_baseUrl}/api/admin/users/{id}";

                var res = _client.DeleteAsync(url).Result;

                if (res.IsSuccessStatusCode)
                    return new BaseResult { Status = true, Message = "کاربر مدیر با موفقیت حذف شد." };

                var msg = res.Content.ReadAsStringAsync().Result;
                return new BaseResult
                {
                    Status = false,
                    Message = string.IsNullOrWhiteSpace(msg) ? "خطا در حذف کاربر مدیر." : msg
                };
            }
            catch (Exception ex)
            {
                return new BaseResult { Status = false, Message = ex.Message };
            }
        }
    }
}
