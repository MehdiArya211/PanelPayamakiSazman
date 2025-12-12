using DTO.Base;
using DTO.DataTable;
using DTO.Project.SenderNumberAssignment;
using DTO.Project.WebApi;
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

namespace BLL.Project.SenderNumberAssignment
{
    public class SenderNumberAssignmentManager : ISenderNumberAssignmentManager
    {
        private readonly IHttpContextAccessor _httpContext;
        private readonly HttpClient _client;
        private readonly string _baseUrl;

        public SenderNumberAssignmentManager(IHttpContextAccessor accessor, IConfiguration config)
        {
            _httpContext = accessor;
            _client = new HttpClient();
            _baseUrl = "http://87.107.111.44:8010"; // یا config["ApiBaseUrl"]
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

        /* ----------------------------------------------
         *  GetAll (اختیاری)
         * ----------------------------------------------*/
        public List<SenderNumberAssignmentListDTO> GetAll()
        {
            try
            {
                SetAuth();

                var url = $"{_baseUrl}/api/admin/sender-number-assignments/search";

                var body = new
                {
                    page = 1,
                    pageSize = 1000,
                    filters = new List<object>(),
                    sortBy = "assignedAt",
                    sortDescending = true
                };

                var jsonBody = JsonSerializer.Serialize(body, JsonOptions);
                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                var res = _client.PostAsync(url, content).Result;
                var jsonResult = res.Content.ReadAsStringAsync().Result;

                if (!res.IsSuccessStatusCode || string.IsNullOrWhiteSpace(jsonResult))
                    return new List<SenderNumberAssignmentListDTO>();

                var apiResponse =
                    JsonSerializer.Deserialize<ApiResponsePagedDTO<SenderNumberAssignmentListDTO>>(jsonResult, JsonOptions);

                return apiResponse?.Data ?? new List<SenderNumberAssignmentListDTO>();
            }
            catch
            {
                return new List<SenderNumberAssignmentListDTO>();
            }
        }

        /* ----------------------------------------------
         *  DataTable
         * ----------------------------------------------*/
        public DataTableResponseDTO<SenderNumberAssignmentListDTO> GetDataTableDTO(DataTableSearchDTO search)
        {
            try
            {
                SetAuth();

                var url = $"{_baseUrl}/api/admin/sender-number-assignments/search";

                var body = new
                {
                    page = (search.start / search.length) + 1,
                    pageSize = search.length,
                    filters = new List<object>(),
                    sortBy = string.IsNullOrWhiteSpace(search.sortColumnName) ? "assignedAt" : search.sortColumnName,
                    sortDescending = search.sortDirection == "desc"
                };

                var jsonBody = JsonSerializer.Serialize(body, JsonOptions);
                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                var res = _client.PostAsync(url, content).Result;
                var jsonResult = res.Content.ReadAsStringAsync().Result;

                if (!res.IsSuccessStatusCode || string.IsNullOrWhiteSpace(jsonResult))
                {
                    return new DataTableResponseDTO<SenderNumberAssignmentListDTO>
                    {
                        draw = search.draw,
                        recordsTotal = 0,
                        recordsFiltered = 0,
                        data = new List<SenderNumberAssignmentListDTO>(),
                        AdditionalData = jsonResult
                    };
                }

                var apiResponse =
                    JsonSerializer.Deserialize<ApiResponsePagedDTO<SenderNumberAssignmentListDTO>>(jsonResult, JsonOptions);

                var total = apiResponse?.Meta?.TotalCount ?? 0;
                var data = apiResponse?.Data ?? new List<SenderNumberAssignmentListDTO>();

                return new DataTableResponseDTO<SenderNumberAssignmentListDTO>
                {
                    draw = search.draw,
                    recordsTotal = total,
                    recordsFiltered = total,
                    data = data
                };
            }
            catch (Exception ex)
            {
                return new DataTableResponseDTO<SenderNumberAssignmentListDTO>
                {
                    draw = search.draw,
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = new List<SenderNumberAssignmentListDTO>(),
                    AdditionalData = ex.Message
                };
            }
        }

        /* ----------------------------------------------
         *  GetById (برای فرم ویرایش توضیحات)
         *  توجه: GET این سرویس Wrapper ندارد؛ مستقیم DTO برمی‌گرداند.
         * ----------------------------------------------*/
        public SenderNumberAssignmentEditDTO GetById(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                    return null;

                SetAuth();

                var url = $"{_baseUrl}/api/admin/sender-number-assignments/{id}";

                var res = _client.GetAsync(url).Result;
                if (!res.IsSuccessStatusCode)
                    return null;

                var jsonResult = res.Content.ReadAsStringAsync().Result;
                if (string.IsNullOrWhiteSpace(jsonResult))
                    return null;

                // مدل کامل API
                var apiModel =
                    JsonSerializer.Deserialize<SenderNumberAssignmentListDTO>(
                        jsonResult, JsonOptions);

                if (apiModel == null)
                    return null;

                return new SenderNumberAssignmentEditDTO
                {
                    Id = apiModel.Id,
                    Description = apiModel.Description
                };
            }
            catch
            {
                return null;
            }
        }

        /* ----------------------------------------------
         *  Create
         * ----------------------------------------------*/
        public BaseResult Create(SenderNumberAssignmentCreateDTO model)
        {
            try
            {
                SetAuth();

                var url = $"{_baseUrl}/api/admin/sender-number-assignments";

                var body = new
                {
                    senderNumberId = model.SenderNumberId,
                    userId = model.UserId,
                    clientId = string.IsNullOrWhiteSpace(model.ClientId) ? null : model.ClientId,
                    description = model.Description
                };

                var jsonBody = JsonSerializer.Serialize(body, JsonOptions);
                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                var res = _client.PostAsync(url, content).Result;
                var msg = res.Content.ReadAsStringAsync().Result;

                if (res.IsSuccessStatusCode)
                    return new BaseResult { Status = true, Message = "تخصیص سرشماره با موفقیت ثبت شد." };

                return new BaseResult
                {
                    Status = false,
                    Message = string.IsNullOrWhiteSpace(msg) ? "خطا در ایجاد تخصیص سرشماره." : msg
                };
            }
            catch (Exception ex)
            {
                return new BaseResult { Status = false, Message = ex.Message };
            }
        }

        /* ----------------------------------------------
         *  Update (فقط توضیحات)
         * ----------------------------------------------*/
        public BaseResult Update(SenderNumberAssignmentEditDTO model)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(model.Id))
                    return new BaseResult { Status = false, Message = "شناسه تخصیص نامعتبر است." };

                SetAuth();

                var url = $"{_baseUrl}/api/admin/sender-number-assignments/{model.Id}";

                var body = new
                {
                    description = model.Description
                };

                var jsonBody = JsonSerializer.Serialize(body, JsonOptions);
                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                var res = _client.PutAsync(url, content).Result;
                var msg = res.Content.ReadAsStringAsync().Result;

                if (res.IsSuccessStatusCode)
                    return new BaseResult { Status = true, Message = "توضیحات تخصیص با موفقیت ویرایش شد." };

                return new BaseResult
                {
                    Status = false,
                    Message = string.IsNullOrWhiteSpace(msg) ? "خطا در ویرایش تخصیص." : msg
                };
            }
            catch (Exception ex)
            {
                return new BaseResult { Status = false, Message = ex.Message };
            }
        }

        /* ----------------------------------------------
         *  Revoke
         * ----------------------------------------------*/
        public BaseResult Revoke(string id, string reason)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                    return new BaseResult { Status = false, Message = "شناسه تخصیص نامعتبر است." };

                SetAuth();

                var url = $"{_baseUrl}/api/admin/sender-number-assignments/{id}/revoke";

                var body = new
                {
                    reason = string.IsNullOrWhiteSpace(reason) ? null : reason
                };

                var jsonBody = JsonSerializer.Serialize(body, JsonOptions);
                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                var res = _client.PutAsync(url, content).Result;
                var msg = res.Content.ReadAsStringAsync().Result;

                if (res.IsSuccessStatusCode)
                    return new BaseResult { Status = true, Message = "دسترسی کاربر از سرشماره با موفقیت لغو شد." };

                return new BaseResult
                {
                    Status = false,
                    Message = string.IsNullOrWhiteSpace(msg) ? "خطا در لغو تخصیص سرشماره." : msg
                };
            }
            catch (Exception ex)
            {
                return new BaseResult { Status = false, Message = ex.Message };
            }
        }

        public List<LookupItemDTO> GetSenderNumberLookup()
        {
            var result = new List<LookupItemDTO>();

            try
            {
                SetAuth();

                var url = $"{_baseUrl}/api/admin/sender-numbers/search";

                var body = new
                {
                    page = 1,
                    pageSize = 200,
                    filters = new List<object>(),
                    sortBy = "fixedPrefix",
                    sortDescending = false
                };

                var jsonBody = JsonSerializer.Serialize(body, JsonOptions);
                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                var res = _client.PostAsync(url, content).Result;
                var jsonResult = res.Content.ReadAsStringAsync().Result;

                if (!res.IsSuccessStatusCode || string.IsNullOrWhiteSpace(jsonResult))
                    return result;

                var apiResponse =
                    JsonSerializer.Deserialize<ApiResponsePagedDTO<SenderNumberLookupApiDTO>>(
                        jsonResult, JsonOptions);

                var data = apiResponse?.Data ?? new List<SenderNumberLookupApiDTO>();

                foreach (var item in data)
                {
                    if (string.IsNullOrWhiteSpace(item.Id))
                        continue;

                    var text = string.IsNullOrWhiteSpace(item.FullNumber)
                        ? item.Id
                        : item.FullNumber;

                    result.Add(new LookupItemDTO
                    {
                        Id = item.Id,
                        Text = text
                    });
                }

                return result;
            }
            catch
            {
                return result;
            }
        }

        public List<LookupItemDTO> GetUserLookup()
        {
            var result = new List<LookupItemDTO>();

            try
            {
                SetAuth();

                // TODO: اگر مستند API اسم دیگری داشت، این URL را اصلاح کن
                var url = $"{_baseUrl}/api/admin/users/search";

                var body = new
                {
                    page = 1,
                    pageSize = 200,
                    filters = new List<object>(),
                    sortBy = "userName",
                    sortDescending = false
                };

                var jsonBody = JsonSerializer.Serialize(body, JsonOptions);
                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                var res = _client.PostAsync(url, content).Result;
                var jsonResult = res.Content.ReadAsStringAsync().Result;

                if (!res.IsSuccessStatusCode || string.IsNullOrWhiteSpace(jsonResult))
                    return result;

                var apiResponse =
                    JsonSerializer.Deserialize<ApiResponsePagedDTO<UserLookupApiDTO>>(
                        jsonResult, JsonOptions);

                var data = apiResponse?.Data ?? new List<UserLookupApiDTO>();

                foreach (var item in data)
                {
                    if (string.IsNullOrWhiteSpace(item.Id))
                        continue;

                    string text;

                    if (!string.IsNullOrWhiteSpace(item.FullName))
                        text = $"{item.FullName} ({item.UserName})";
                    else
                        text = item.UserName ?? item.Id;

                    result.Add(new LookupItemDTO
                    {
                        Id = item.Id,
                        Text = text
                    });
                }

                return result;
            }
            catch
            {
                return result;
            }
        }

        public List<LookupItemDTO> GetClientLookup()
        {
            var result = new List<LookupItemDTO>();

            try
            {
                SetAuth();

                // TODO: اگر مستند API چیز دیگری بود، این URL را اصلاح کن
                var url = $"{_baseUrl}/api/admin/clients/search";

                var body = new
                {
                    page = 1,
                    pageSize = 200,
                    filters = new List<object>(),
                    sortBy = "name",
                    sortDescending = false
                };

                var jsonBody = JsonSerializer.Serialize(body, JsonOptions);
                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                var res = _client.PostAsync(url, content).Result;
                var jsonResult = res.Content.ReadAsStringAsync().Result;

                if (!res.IsSuccessStatusCode || string.IsNullOrWhiteSpace(jsonResult))
                    return result;

                var apiResponse =
                    JsonSerializer.Deserialize<ApiResponsePagedDTO<ClientLookupApiDTO>>(
                        jsonResult, JsonOptions);

                var data = apiResponse?.Data ?? new List<ClientLookupApiDTO>();

                foreach (var item in data)
                {
                    if (string.IsNullOrWhiteSpace(item.Id))
                        continue;

                    string text;

                    if (!string.IsNullOrWhiteSpace(item.Code))
                        text = $"{item.Code} - {item.Name}";
                    else
                        text = item.Name ?? item.Id;

                    result.Add(new LookupItemDTO
                    {
                        Id = item.Id,
                        Text = text
                    });
                }

                return result;
            }
            catch
            {
                return result;
            }

        }

    }
    // فقط برای lookup سرشماره‌ها
    public class SenderNumberLookupApiDTO
    {
        public string Id { get; set; }
        public string FullNumber { get; set; }
        public string FixedPrefix { get; set; }
    }

    // فقط برای lookup کاربران
    public class UserLookupApiDTO
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
    }

    // فقط برای lookup کلاینت‌ها
    public class ClientLookupApiDTO
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
    }
}
