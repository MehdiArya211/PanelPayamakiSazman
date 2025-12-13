using DTO.Base;
using DTO.Project.RoleAssignment;
using DTO.WebApi;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace BLL.Project.RoleAssignment
{
    public class RoleAssignmentManager : IRoleAssignmentManager
    {
        private readonly IHttpContextAccessor _httpContext;
        private readonly HttpClient _client;
        private readonly string _baseUrl;

        public RoleAssignmentManager(IHttpContextAccessor accessor, IConfiguration config)
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

        public BaseResult AssignToUser(AssignRoleToUserDTO model)
        {
            try
            {
                SetAuth();

                var url = $"{_baseUrl}/role-assignments/assign";

                var json = JsonSerializer.Serialize(model);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var res = _client.PostAsync(url, content).Result;

                if (res.IsSuccessStatusCode)
                    return new BaseResult(true, "نقش با موفقیت به کاربر تخصیص داده شد.");

                return new BaseResult(false, res.Content.ReadAsStringAsync().Result);
            }
            catch (Exception ex)
            {
                return new BaseResult(false, ex.Message);
            }
        }

        public BaseResult ReplaceUserRoles(Guid userId, List<Guid> roleIds)
        {
            try
            {
                SetAuth();

                var url = $"{_baseUrl}/role-assignments/users/{userId}/roles";

                var body = new
                {
                    roleIds = roleIds
                };

                var json = JsonSerializer.Serialize(body);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var res = _client.PutAsync(url, content).Result;

                if (res.IsSuccessStatusCode)
                    return new BaseResult(true, "نقش‌های کاربر با موفقیت بروزرسانی شد.");

                return new BaseResult(false, res.Content.ReadAsStringAsync().Result);
            }
            catch (Exception ex)
            {
                return new BaseResult(false, ex.Message);
            }
        }

        public BaseResult AssignToUnit(Guid unitId, Guid roleId)
        {
            try
            {
                SetAuth();

                var url = $"{_baseUrl}/role-assignments/units/{unitId}/assign";

                var body = new { roleId };

                var json = JsonSerializer.Serialize(body);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var res = _client.PostAsync(url, content).Result;

                if (res.IsSuccessStatusCode)
                    return new BaseResult(true, "نقش با موفقیت به واحد سازمانی تخصیص داده شد.");

                return new BaseResult(false, res.Content.ReadAsStringAsync().Result);
            }
            catch (Exception ex)
            {
                return new BaseResult(false, ex.Message);
            }


        }


        /// <summary>
        /// این متد فقط در صورتی کار می‌کند که سمت API یک GET برای نقش‌های کاربر وجود داشته باشد.
        /// اگر نبود، لیست خالی برمی‌گرداند ولی بقیه عملیات (Assign/Replace) بدون مشکل کار می‌کنند.
        /// </summary>
        public List<Guid> GetUserRoleIds(Guid userId)
        {
            try
            {
                SetAuth();

                // گزینه محتمل ۱ (اگر API داشته باشد)
                var url1 = $"{_baseUrl}/role-assignments/users/{userId}/roles";
                var res1 = _client.GetAsync(url1).Result;

                if (res1.IsSuccessStatusCode)
                {
                    var json1 = res1.Content.ReadAsStringAsync().Result;

                    // حالت A: wrapper با Data
                    var wrapped = JsonSerializer.Deserialize<ApiResponseListDTO<UserRoleIdsDTO>>(
                        json1,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                    );
                    if (wrapped?.Data?.RoleIds != null)
                        return wrapped.Data.RoleIds;

                    // حالت B: مستقیم
                    var direct = JsonSerializer.Deserialize<UserRoleIdsDTO>(
                        json1,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                    );
                    return direct?.RoleIds ?? new List<Guid>();
                }

                return new List<Guid>();
            }
            catch
            {
                return new List<Guid>();
            }
        }
    }
}

