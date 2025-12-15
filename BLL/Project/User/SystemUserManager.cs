using BLL.Project.SenderNumberAssignment;
using DTO.Base;
using DTO.DataTable;
using DTO.Project.SecurityQuestion;
using DTO.Project.SenderNumberSubAreaList;
using DTO.Project.User;
using DTO.Project.WebApi;
using DTO.WebApi;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using static System.Net.WebRequestMethods;

namespace BLL.Project.User
{
    public class SystemUserManager : ISystemUserManager
    {
        private readonly IHttpContextAccessor _httpContext;
        private readonly HttpClient _client;
        private readonly string _baseUrl;


        public SystemUserManager(IHttpContextAccessor accessor, IConfiguration config)
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

        public List<SystemUserListDTO> GetAll0()
        {
            SetAuth();

            var url = $"{_baseUrl}/users";
            var res = _client.GetAsync(url).Result;

            var json = res.Content.ReadAsStringAsync().Result;

            return JsonSerializer.Deserialize<List<SystemUserListDTO>>(
                json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            ) ?? new List<SystemUserListDTO>();
        }
        public List<LookupItemDTO> GetUserLookup()
        {
            var result = new List<LookupItemDTO>();

            try
            {
                SetAuth();

                var url = $"{_baseUrl}/users/search";

                var body = new
                {
                    page = 1,
                    pageSize = 200,
                    filters = new List<object>(),
                    sortBy = "userName",
                    sortDescending = false
                };

                var json = JsonSerializer.Serialize(body);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var res = _client.PostAsync(url, content).Result;
                var jsonResult = res.Content.ReadAsStringAsync().Result;

                if (!res.IsSuccessStatusCode)
                    return result;

                var api =
                    JsonSerializer.Deserialize<ApiResponsePagedDTO<UserLookupApiDTO>>(jsonResult,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                foreach (var u in api?.Data ?? new())
                {
                    result.Add(new LookupItemDTO
                    {
                        Id = u.Id,
                        Text = string.IsNullOrWhiteSpace(u.FullName)
                            ? u.UserName
                            : $"{u.FullName} ({u.UserName})"
                    });
                }
            }
            catch { }

            return result;
        }






        public DataTableResponseDTO<SystemUserListDTO> GetDataTable(DataTableSearchDTO search)
        {
            SetAuth();

           // var url = "http://87.107.111.44:8010/api/admin/users/search";
            var url = $"{_baseUrl}/users/search";

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
            var responseJson = res.Content.ReadAsStringAsync().Result;

            if (!res.IsSuccessStatusCode)
            {
                return new DataTableResponseDTO<SystemUserListDTO>
                {
                    draw = search.draw,
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = new List<SystemUserListDTO>(),

                };
            }

            var apiResponse = JsonSerializer.Deserialize<ApiResponsePagedDTO<SystemUserListDTO>>(
                responseJson,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );

            return new DataTableResponseDTO<SystemUserListDTO>
            {
                draw = search.draw,
                recordsTotal = apiResponse.Meta.TotalCount,
                recordsFiltered = apiResponse.Meta.TotalCount,
                data = apiResponse.Data
            };
        }

        public BaseResult Create0(SystemUserCreateDTO model)
        {
            try
            {
                SetAuth();

               // var url = "http://87.107.111.44:8010/api/admin/users";
                var url = $"{_baseUrl}/users";

                var json = JsonSerializer.Serialize(model);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var res = _client.PostAsync(url, content).Result;

                if (res.IsSuccessStatusCode)
                    return new BaseResult(true, "کاربر با موفقیت ثبت شد.");

                return new BaseResult(false, res.Content.ReadAsStringAsync().Result);
            }
            catch (Exception ex)
            {
                return new BaseResult(false, ex.Message);
            }
        }

        public BaseResult Create1(SystemUserCreateDTO model)
        {
            try
            {
                SetAuth();

                //var url = "http://87.107.111.44:8010/api/admin/users";
                var url = $"{_baseUrl}/users";

                var body = new
                {
                    unitId = model.UnitId,
                    userName = model.UserName,
                    initialPassword = model.InitialPassword,
                    firstName = model.FirstName,
                    lastName = model.LastName,
                    nationalCode = model.NationalCode,
                    mobileNumber = model.MobileNumber,

                    //securityQuestions = model.SecurityQuestions != null
                    //    ? model.SecurityQuestions.Select(q => new SecurityQuestionBodyDTO
                    //    {
                    //        QuestionId = q.id,
                    //        Answer = q.Title
                    //    }).ToList()
                    //    : new List<SecurityQuestionBodyDTO>(),

                    roleIds = (model.RoleIds != null && model.RoleIds.Any())
                        ? model.RoleIds
                        : null
                };


                var json = JsonSerializer.Serialize(body);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var res = _client.PostAsync(url, content).Result;

                if (res.IsSuccessStatusCode)
                    return new BaseResult(true, "کاربر با موفقیت ثبت شد.");

                var errorJson = res.Content.ReadAsStringAsync().Result;
                return new BaseResult(false, errorJson);
            }
            catch (Exception ex)
            {
                return new BaseResult(false, ex.Message);
            }
        }

        public BaseResult Create(SystemUserCreateDTO model)
        {
            try
            {
                SetAuth();

                var url = $"{_baseUrl}/users";

                var body = new
                {
                    unitId = model.UnitId,
                   // unitId = "019b20ba-70a6-772b-8a8b-eca9f1b17768",
                    userName = model.UserName,
                    initialPassword = model.InitialPassword,
                    firstName = model.FirstName,
                    lastName = model.LastName,
                    nationalCode = model.NationalCode,
                    mobileNumber = model.MobileNumber,
                    roleIds = model.RoleIds != null && model.RoleIds.Any()
                        ? model.RoleIds
                        : new List<Guid>()
                };

                var json = JsonSerializer.Serialize(body);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var res = _client.PostAsync(url, content).Result;
                var responseBody = res.Content.ReadAsStringAsync().Result;

                if (res.IsSuccessStatusCode)
                    return new BaseResult(true, "کاربر با موفقیت ثبت شد.");

                // 👇 Decode حرفه‌ای خطا
                var message = ParseApiError(res.StatusCode, responseBody);
                return new BaseResult(false, message);
            }
            catch (Exception ex)
            {
                return new BaseResult(false, $"خطای داخلی: {ex.Message}");
            }
        }

        private string ParseApiError(HttpStatusCode statusCode, string responseBody)
        {
            try
            {
                // Validation Error (ProblemDetails)
                if (responseBody.StartsWith("{") && responseBody.Contains("\"errors\""))
                {
                    using var doc = JsonDocument.Parse(responseBody);

                    if (doc.RootElement.TryGetProperty("errors", out var errors))
                    {
                        var messages = new List<string>();

                        foreach (var field in errors.EnumerateObject())
                        {
                            foreach (var err in field.Value.EnumerateArray())
                            {
                                messages.Add(err.GetString());
                            }
                        }

                        return string.Join("<br/>", messages);
                    }
                }

                // fallback
                return $"خطای API ({(int)statusCode}): {responseBody}";
            }
            catch
            {
                return $"خطای API ({(int)statusCode})";
            }
        }


        public SystemUserEditDTO GetById(string id)
        {
            try
            {
                SetAuth();

                //var url = $"http://87.107.111.44:8010/api/admin/users/{id}";
                var url = $"{_baseUrl}/users/{id}";

                var res = _client.GetAsync(url).Result;

                if (!res.IsSuccessStatusCode)
                    return null;

                var json = res.Content.ReadAsStringAsync().Result;

                var apiResponse =
                    JsonSerializer.Deserialize<ApiResponseSingleDTO<SystemUserEditDTO>>(
                        json,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                    );

                return apiResponse.Data;
            }
            catch
            {
                return null;
            }
        }

        public BaseResult Update(SystemUserEditDTO model)
        {
            try
            {
                SetAuth();

                //var url = $"http://87.107.111.44:8010/api/admin/users/{model.Id}";
                var url = $"{_baseUrl}/users/{model.Id}";

                var json = JsonSerializer.Serialize(model);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var res = _client.PutAsync(url, content).Result;

                if (res.IsSuccessStatusCode)
                    return new BaseResult(true, "ویرایش با موفقیت انجام شد.");

                return new BaseResult(false, res.Content.ReadAsStringAsync().Result);
            }
            catch (Exception ex)
            {
                return new BaseResult(false, ex.Message);
            }
        }

        public BaseResult Delete(string id)
        {
            try
            {
                SetAuth();

               // var url = $"http://87.107.111.44:8010/api/admin/users/{id}";
                var url = $"{_baseUrl}/users/{id}";
                var res = _client.DeleteAsync(url).Result;

                if (res.IsSuccessStatusCode)
                    return new BaseResult(true, "کاربر حذف شد.");

                return new BaseResult(false, res.Content.ReadAsStringAsync().Result);
            }
            catch (Exception ex)
            {
                return new BaseResult(false, ex.Message);
            }
        }
    }
}
