using DTO.Base;
using DTO.DataTable;
using DTO.Project.SmsManager;
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

namespace BLL.Project.SmsManager
{
    public class AdminSmsManager : IAdminSmsManager
    {
        private readonly IHttpContextAccessor _httpContext;
        private readonly HttpClient _client;
        private readonly string _baseUrl;

        public AdminSmsManager(IHttpContextAccessor accessor, IConfiguration config)
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
         *  مدل داخلی برای lookup سرشماره‌ها
         * ----------------------------------------------*/
        private class SenderNumberLookupApiDTO
        {
            public string Id { get; set; }
            public string FullNumber { get; set; }
            public string FixedPrefix { get; set; }
        }

        /* ----------------------------------------------
         *  مدل داخلی برای lookup گروه مخاطبین (URL فرضی)
         * ----------------------------------------------*/
        private class ContactGroupLookupApiDTO
        {
            public string Id { get; set; }
            public string Name { get; set; }
        }

        /* ----------------------------------------------
         *  تاریخچه پیامک‌ها - خروجی دیتاتیبل
         * ----------------------------------------------*/
        public DataTableResponseDTO<AdminSmsMessageListDTO> GetHistory(
            AdminSmsHistoryFilterDTO filter,
            DataTableSearchDTO search)
        {
            try
            {
                SetAuth();

                var url = $"{_baseUrl}/api/admin/sms/messages/history/search";

                var paging = new
                {
                    page = (search.start / search.length) + 1,
                    pageSize = search.length,
                    filters = new List<object>(),
                    sortBy = string.IsNullOrWhiteSpace(search.sortColumnName) ? "createdOn" : search.sortColumnName,
                    sortDescending = search.sortDirection == "desc"
                };

                var body = new
                {
                    search = string.IsNullOrWhiteSpace(filter.Search) ? null : filter.Search,
                    senderNumberId = string.IsNullOrWhiteSpace(filter.SenderNumberId) ? null : filter.SenderNumberId,
                    batchId = string.IsNullOrWhiteSpace(filter.BatchId) ? null : filter.BatchId,
                    requestedByUserId = string.IsNullOrWhiteSpace(filter.RequestedByUserId) ? null : filter.RequestedByUserId,
                    excludeRequestedByUserId = string.IsNullOrWhiteSpace(filter.ExcludeRequestedByUserId) ? null : filter.ExcludeRequestedByUserId,
                    status = string.IsNullOrWhiteSpace(filter.Status) ? null : filter.Status,
                    channel = string.IsNullOrWhiteSpace(filter.Channel) ? null : filter.Channel,
                    from = filter.From,
                    to = filter.To,
                    onlyOutbound = filter.OnlyOutbound,
                    paging = paging
                };

                var jsonBody = JsonSerializer.Serialize(body, JsonOptions);
                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                var res = _client.PostAsync(url, content).Result;
                var jsonResult = res.Content.ReadAsStringAsync().Result;

                if (!res.IsSuccessStatusCode || string.IsNullOrWhiteSpace(jsonResult))
                {
                    return new DataTableResponseDTO<AdminSmsMessageListDTO>
                    {
                        draw = search.draw,
                        recordsTotal = 0,
                        recordsFiltered = 0,
                        data = new List<AdminSmsMessageListDTO>(),
                        AdditionalData = jsonResult
                    };
                }

                var apiResponse =
                    JsonSerializer.Deserialize<AdminSmsHistoryResponseDTO<AdminSmsMessageListDTO>>(
                        jsonResult, JsonOptions);

                var total = apiResponse?.TotalCount ?? 0;
                var data = apiResponse?.Items ?? new List<AdminSmsMessageListDTO>();

                return new DataTableResponseDTO<AdminSmsMessageListDTO>
                {
                    draw = search.draw,
                    recordsTotal = total,
                    recordsFiltered = total,
                    data = data
                };
            }
            catch (Exception ex)
            {
                return new DataTableResponseDTO<AdminSmsMessageListDTO>
                {
                    draw = search.draw,
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = new List<AdminSmsMessageListDTO>(),
                    AdditionalData = ex.Message
                };
            }
        }

        /* ----------------------------------------------
         *  ارسال تکی
         * ----------------------------------------------*/
        public BaseResult SendSingle(SmsSingleSendDTO model)
        {
            try
            {
                SetAuth();

                var url = $"{_baseUrl}/api/admin/sms/messages/single";

                var body = new
                {
                    senderNumberId = model.SenderNumberId,
                    receiverNumber = model.ReceiverNumber,
                    text = model.Text
                };

                var jsonBody = JsonSerializer.Serialize(body, JsonOptions);
                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                var res = _client.PostAsync(url, content).Result;
                var msg = res.Content.ReadAsStringAsync().Result;

                if (res.IsSuccessStatusCode)
                    return new BaseResult { Status = true, Message = "پیامک با موفقیت در صف ارسال ثبت شد." };

                return new BaseResult
                {
                    Status = false,
                    Message = string.IsNullOrWhiteSpace(msg) ? "خطا در ارسال پیامک تکی." : msg
                };
            }
            catch (Exception ex)
            {
                return new BaseResult { Status = false, Message = ex.Message };
            }
        }

        /* ----------------------------------------------
         *  ارسال گروهی (لیست شماره‌ها)
         * ----------------------------------------------*/
        public BaseResult SendBulk(SmsBulkSendDTO model)
        {
            try
            {
                SetAuth();

                var url = $"{_baseUrl}/api/admin/sms/messages/bulk";

                var body = new
                {
                    senderNumberId = model.SenderNumberId,
                    receiverNumbers = model.ReceiverNumbers,
                    text = model.Text
                };

                var jsonBody = JsonSerializer.Serialize(body, JsonOptions);
                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                var res = _client.PostAsync(url, content).Result;
                var msg = res.Content.ReadAsStringAsync().Result;

                if (res.IsSuccessStatusCode)
                    return new BaseResult { Status = true, Message = "پیامک‌های گروهی در صف ارسال ثبت شدند." };

                return new BaseResult
                {
                    Status = false,
                    Message = string.IsNullOrWhiteSpace(msg) ? "خطا در ارسال پیامک‌های گروهی." : msg
                };
            }
            catch (Exception ex)
            {
                return new BaseResult { Status = false, Message = ex.Message };
            }
        }

        /* ----------------------------------------------
         *  ارسال به گروه‌های مخاطب
         * ----------------------------------------------*/
        public BaseResult SendToGroups(SmsGroupSendDTO model)
        {
            try
            {
                SetAuth();

                var url = $"{_baseUrl}/api/admin/sms/messages/groups";

                var body = new
                {
                    senderNumberId = model.SenderNumberId,
                    contactGroupIds = model.ContactGroupIds,
                    text = model.Text
                };

                var jsonBody = JsonSerializer.Serialize(body, JsonOptions);
                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                var res = _client.PostAsync(url, content).Result;
                var msg = res.Content.ReadAsStringAsync().Result;

                if (res.IsSuccessStatusCode)
                    return new BaseResult { Status = true, Message = "پیامک‌ها برای گروه‌های انتخاب‌شده ثبت شدند." };

                return new BaseResult
                {
                    Status = false,
                    Message = string.IsNullOrWhiteSpace(msg) ? "خطا در ارسال پیامک به گروه‌ها." : msg
                };
            }
            catch (Exception ex)
            {
                return new BaseResult { Status = false, Message = ex.Message };
            }
        }

        /* ----------------------------------------------
         *  ارسال از فایل (CSV/Excel)
         * ----------------------------------------------*/
        public BaseResult SendFromFile(SmsFileSendViewModel model)
        {
            try
            {
                SetAuth();

                var url = $"{_baseUrl}/api/admin/sms/messages/file";

                using var content = new MultipartFormDataContent();

                content.Add(new StringContent(model.SenderNumberId), "senderNumberId");
                content.Add(new StringContent(model.Text ?? string.Empty), "text");

                if (model.File != null && model.File.Length > 0)
                {
                    var fileContent = new StreamContent(model.File.OpenReadStream());
                    fileContent.Headers.ContentType =
                        new System.Net.Http.Headers.MediaTypeHeaderValue(model.File.ContentType);

                    content.Add(fileContent, "file", model.File.FileName);
                }

                var res = _client.PostAsync(url, content).Result;
                var msg = res.Content.ReadAsStringAsync().Result;

                if (res.IsSuccessStatusCode)
                    return new BaseResult { Status = true, Message = "فایل دریافت شد و پیامک‌ها ثبت شدند." };

                return new BaseResult
                {
                    Status = false,
                    Message = string.IsNullOrWhiteSpace(msg) ? "خطا در ارسال پیامک از فایل." : msg
                };
            }
            catch (Exception ex)
            {
                return new BaseResult { Status = false, Message = ex.Message };
            }
        }

        /* ----------------------------------------------
         *  Lookup سرشماره‌ها
         * ----------------------------------------------*/
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

        /* ----------------------------------------------
         *  Lookup گروه مخاطبین
         *  توجه: URL زیر فرضی است؛ باید بر اساس مستند واقعی اصلاح شود.
         * ----------------------------------------------*/
        public List<LookupItemDTO> GetContactGroupLookup()
        {
            var result = new List<LookupItemDTO>();

            try
            {
                SetAuth();

                // TODO: اگر مستند API نام دیگری دارد، این URL را اصلاح کن
                var url = $"{_baseUrl}/api/admin/contact-groups/search";

                var body = new
                {
                    page = 1,
                    pageSize = 1000,
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
                    JsonSerializer.Deserialize<ApiResponsePagedDTO<ContactGroupLookupApiDTO>>(
                        jsonResult, JsonOptions);

                var data = apiResponse?.Data ?? new List<ContactGroupLookupApiDTO>();

                foreach (var item in data)
                {
                    if (string.IsNullOrWhiteSpace(item.Id))
                        continue;

                    var text = item.Name ?? item.Id;

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
}
