using DTO.Base;
using DTO.DataTable;
using DTO.Project.SmsConsumer;
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

namespace BLL.Project.SmsConsumer
{
    public class SmsConsumerManager : ISmsConsumerManager
    {
        private readonly IHttpContextAccessor _httpContext;
        private readonly HttpClient _client;
        private readonly string _baseUrl;

        public SmsConsumerManager(IHttpContextAccessor accessor, IConfiguration config)
        {
            _httpContext = accessor;
            _client = new HttpClient();
            //_baseUrl = config["ApiBaseUrl"] ?? "http://87.107.111.44:8010";
            _baseUrl = "http://87.107.111.44:8010";
        }

        private void SetAuth()
        {
            // اگر توکن consumer داری، همینجا اسمش رو درست کن
            var token = _httpContext.HttpContext.Session.GetString("AdminToken");

            _client.DefaultRequestHeaders.Authorization = null;
            if (!string.IsNullOrWhiteSpace(token))
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        private JsonSerializerOptions JsonOptions => new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        /* ===========================
           HISTORY
        =========================== */
        public DataTableResponseDTO<SmsMessageHistoryListDTO> GetHistoryDataTableDTO(DataTableSearchDTO search, bool onlyOutbound)
        {
            SetAuth();

            var url = $"{_baseUrl}/api/sms/messages/history/search?onlyOutbound={onlyOutbound.ToString().ToLower()}";

            var body = new
            {
                page = (search.start / search.length) + 1,
                pageSize = search.length,
                filters = new List<object>(),
                sortBy = string.IsNullOrWhiteSpace(search.sortColumnName) ? null : search.sortColumnName,
                sortDescending = search.sortDirection == "desc"
            };

            var json = JsonSerializer.Serialize(body, JsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var res = _client.PostAsync(url, content).Result;
            var jsonResult = res.Content.ReadAsStringAsync().Result;

            if (!res.IsSuccessStatusCode || string.IsNullOrWhiteSpace(jsonResult))
            {
                return new DataTableResponseDTO<SmsMessageHistoryListDTO>
                {
                    draw = search.draw,
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = new List<SmsMessageHistoryListDTO>(),
                    AdditionalData = jsonResult
                };
            }

            var apiResponse = JsonSerializer.Deserialize<ApiResponsePagedDTO<SmsMessageHistoryListDTO>>(jsonResult, JsonOptions);

            var total = apiResponse?.Meta?.TotalCount ?? 0;
            var data = apiResponse?.Data ?? new List<SmsMessageHistoryListDTO>();

            // senderNumberId => fullNumber
            var snMap = GetSenderNumberOptions()
                .GroupBy(x => x.Id)
                .ToDictionary(g => g.Key, g => g.First().Display);

            foreach (var m in data)
            {
                if (!string.IsNullOrWhiteSpace(m.SenderNumberId) && snMap.TryGetValue(m.SenderNumberId, out var display))
                    m.SenderFullNumber = display;
                else
                    m.SenderFullNumber = m.SenderNumberId;
            }

            return new DataTableResponseDTO<SmsMessageHistoryListDTO>
            {
                draw = search.draw,
                recordsTotal = total,
                recordsFiltered = total,
                data = data
            };
        }

        /* ===========================
           SEND SINGLE
        =========================== */
        public BaseResult SendSingle(SmsSendSingleDTO model)
        {
            try
            {
                SetAuth();

                var url = $"{_baseUrl}/api/sms/messages/single";

                var body = new
                {
                    senderNumberId = model.SenderNumberId,
                    receiverNumber = model.ReceiverNumber,
                    text = model.Text
                };

                var json = JsonSerializer.Serialize(body, JsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var res = _client.PostAsync(url, content).Result;

                if (res.IsSuccessStatusCode)
                    return new BaseResult { Status = true, Message = "پیامک تکی ثبت شد." };

                return new BaseResult { Status = false, Message = res.Content.ReadAsStringAsync().Result };
            }
            catch (Exception ex)
            {
                return new BaseResult { Status = false, Message = ex.Message };
            }
        }

        /* ===========================
           SEND BULK
        =========================== */
        public BaseResult SendBulk(string senderNumberId, List<string> receiverNumbers, string text)
        {
            try
            {
                SetAuth();

                var url = $"{_baseUrl}/api/sms/messages/bulk";

                var body = new
                {
                    senderNumberId = senderNumberId,
                    receiverNumbers = receiverNumbers,
                    text = text
                };

                var json = JsonSerializer.Serialize(body, JsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var res = _client.PostAsync(url, content).Result;

                if (res.IsSuccessStatusCode)
                    return new BaseResult { Status = true, Message = "ارسال گروهی ثبت شد." };

                return new BaseResult { Status = false, Message = res.Content.ReadAsStringAsync().Result };
            }
            catch (Exception ex)
            {
                return new BaseResult { Status = false, Message = ex.Message };
            }
        }

        /* ===========================
           SEND GROUPS
        =========================== */
        public BaseResult SendGroups(SmsSendGroupsDTO model)
        {
            try
            {
                SetAuth();

                var url = $"{_baseUrl}/api/sms/messages/groups";

                var body = new
                {
                    senderNumberId = model.SenderNumberId,
                    contactGroupIds = model.ContactGroupIds,
                    text = model.Text
                };

                var json = JsonSerializer.Serialize(body, JsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var res = _client.PostAsync(url, content).Result;

                if (res.IsSuccessStatusCode)
                    return new BaseResult { Status = true, Message = "ارسال به گروه‌ها ثبت شد." };

                return new BaseResult { Status = false, Message = res.Content.ReadAsStringAsync().Result };
            }
            catch (Exception ex)
            {
                return new BaseResult { Status = false, Message = ex.Message };
            }
        }

        /* ===========================
           SEND FILE
        =========================== */
        public BaseResult SendFile(SmsSendFileDTO model)
        {
            try
            {
                SetAuth();

                var url = $"{_baseUrl}/api/sms/messages/file";

                using var mp = new MultipartFormDataContent();

                mp.Add(new StringContent(model.SenderNumberId), "senderNumberId");
                mp.Add(new StringContent(model.Text ?? ""), "text");

                if (model.File == null || model.File.Length == 0)
                    return new BaseResult { Status = false, Message = "فایل معتبر نیست." };

                // نکته: stream را dispose نکن تا بعد از ارسال
                var stream = model.File.OpenReadStream();
                var fileContent = new StreamContent(stream);
                fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(model.File.ContentType ?? "application/octet-stream");

                mp.Add(fileContent, "file", model.File.FileName);

                var res = _client.PostAsync(url, mp).Result;

                stream.Dispose(); // بعد از ارسال

                if (res.IsSuccessStatusCode)
                    return new BaseResult { Status = true, Message = "ارسال از فایل ثبت شد." };

                return new BaseResult { Status = false, Message = res.Content.ReadAsStringAsync().Result };
            }
            catch (Exception ex)
            {
                return new BaseResult { Status = false, Message = ex.Message };
            }
        }

        /* ===========================
           OPTIONS
        =========================== */

        public List<SenderNumberOptionDTO> GetSenderNumberOptions()
        {
            try
            {
                SetAuth();

                var url = $"{_baseUrl}/api/consumer/sender-numbers/search";

                var body = new
                {
                    page = 1,
                    pageSize = 200,
                    filters = new List<object>(),
                    sortBy = "id",
                    sortDescending = true
                };

                var json = JsonSerializer.Serialize(body, JsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var res = _client.PostAsync(url, content).Result;
                var jsonResult = res.Content.ReadAsStringAsync().Result;

                if (!res.IsSuccessStatusCode || string.IsNullOrWhiteSpace(jsonResult))
                    return new List<SenderNumberOptionDTO>();

                using var doc = JsonDocument.Parse(jsonResult);
                if (!doc.RootElement.TryGetProperty("data", out var dataEl) || dataEl.ValueKind != JsonValueKind.Array)
                    return new List<SenderNumberOptionDTO>();

                var list = new List<SenderNumberOptionDTO>();

                foreach (var item in dataEl.EnumerateArray())
                {
                    // طبق سرویس consumer sender-numbers: معمولاً senderNumberId + fullNumber
                    var id = item.TryGetProperty("senderNumberId", out var sid) ? sid.ToString()
                           : item.TryGetProperty("id", out var idEl) ? idEl.ToString()
                           : null;

                    var full = item.TryGetProperty("fullNumber", out var fn) ? fn.GetString()
                             : item.TryGetProperty("senderNumber", out var sn) ? sn.GetString()
                             : null;

                    if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(full))
                        continue;

                    list.Add(new SenderNumberOptionDTO { Id = id, Display = full });
                }

                return list.GroupBy(x => x.Id).Select(g => g.First()).ToList();
            }
            catch
            {
                return new List<SenderNumberOptionDTO>();
            }
        }

        public List<ContactGroupOptionDTO> GetContactGroupOptions()
        {
            try
            {
                SetAuth();

                var url = $"{_baseUrl}/api/consumer/contact-groups/search";

                var body = new
                {
                    page = 1,
                    pageSize = 200,
                    filters = new List<object>(),
                    sortBy = "name",
                    sortDescending = false
                };

                var json = JsonSerializer.Serialize(body, JsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var res = _client.PostAsync(url, content).Result;
                var jsonResult = res.Content.ReadAsStringAsync().Result;

                if (!res.IsSuccessStatusCode || string.IsNullOrWhiteSpace(jsonResult))
                    return new List<ContactGroupOptionDTO>();

                using var doc = JsonDocument.Parse(jsonResult);
                if (!doc.RootElement.TryGetProperty("data", out var dataEl) || dataEl.ValueKind != JsonValueKind.Array)
                    return new List<ContactGroupOptionDTO>();

                var list = new List<ContactGroupOptionDTO>();

                foreach (var item in dataEl.EnumerateArray())
                {
                    var id = item.TryGetProperty("id", out var idEl) ? idEl.ToString() : null;
                    var name = item.TryGetProperty("name", out var n) ? n.GetString() : null;
                    if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(name)) continue;

                    list.Add(new ContactGroupOptionDTO { Id = id, Display = name });
                }

                return list.GroupBy(x => x.Id).Select(g => g.First()).ToList();
            }
            catch
            {
                return new List<ContactGroupOptionDTO>();
            }
        }
    }
}
