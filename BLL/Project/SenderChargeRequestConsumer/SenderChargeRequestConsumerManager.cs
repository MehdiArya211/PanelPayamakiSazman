using DTO.Base;
using DTO.DataTable;
using DTO.Project.SenderChargeRequestConsumer;
using DTO.WebApi;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BLL.Project.SenderChargeRequestConsumer
{
    public class SenderChargeRequestConsumerManager : ISenderChargeRequestConsumerManager
    {
        private readonly IHttpContextAccessor _httpContext;
        private readonly HttpClient _client;
        private readonly string _baseUrl;

        public SenderChargeRequestConsumerManager(IHttpContextAccessor accessor)
        {
            _httpContext = accessor;
            _client = new HttpClient();
            _baseUrl = "http://87.107.111.44:8010";
        }

        private void SetAuth()
        {
            // اگر توی consumer توکن جدا داری این کلید را عوض کن
            var token = _httpContext.HttpContext?.Session.GetString("AdminToken");

            _client.DefaultRequestHeaders.Authorization = null;

            if (!string.IsNullOrWhiteSpace(token))
            {
                _client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }
        }

        private JsonSerializerOptions JsonOptions =>
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        public DataTableResponseDTO<SenderChargeRequestListDTO> GetDataTableDTO(DataTableSearchDTO search)
        {
            SetAuth();

            var url = $"{_baseUrl}/api/consumer/sender-charge-requests/search";

            var body = new
            {
                page = (search.start / search.length) + 1,
                pageSize = search.length,
                filters = new List<object>(),
                sortBy = search.sortColumnName,
                sortDescending = search.sortDirection == "desc"
            };

            var json = JsonSerializer.Serialize(body, JsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var res = _client.PostAsync(url, content).Result;
            var jsonResult = res.Content.ReadAsStringAsync().Result;

            if (!res.IsSuccessStatusCode)
            {
                return new DataTableResponseDTO<SenderChargeRequestListDTO>
                {
                    draw = search.draw,
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = new List<SenderChargeRequestListDTO>(),
                    AdditionalData = jsonResult
                };
            }

            var apiResponse =
                JsonSerializer.Deserialize<ApiResponsePagedDTO<SenderChargeRequestListDTO>>(jsonResult, JsonOptions);

            var snOptions = GetSenderNumberOptions();

            // دیکشنری id => display
            var map = snOptions
                .GroupBy(x => x.Id)
                .ToDictionary(g => g.Key, g => g.First().Display);

            foreach (var r in apiResponse.Data)
            {
                if (!string.IsNullOrWhiteSpace(r.SenderNumberId) && map.TryGetValue(r.SenderNumberId, out var display))
                    r.SenderNumberFullNumber = display;
                else
                    r.SenderNumberFullNumber = r.SenderNumberId; // fallback
            }


            var total = apiResponse?.Meta?.TotalCount ?? 0;

            return new DataTableResponseDTO<SenderChargeRequestListDTO>
            {
                draw = search.draw,
                recordsTotal = total,
                recordsFiltered = total,
                data = apiResponse?.Data ?? new List<SenderChargeRequestListDTO>()
            };
        }

        public BaseResult Create(SenderChargeRequestCreateDTO model)
        {
            try
            {
                SetAuth();

                var url = $"{_baseUrl}/api/consumer/sender-charge-requests";

                using var multipart = new MultipartFormDataContent();

                multipart.Add(new StringContent(model.SenderNumberId ?? ""), "senderNumberId");
                multipart.Add(new StringContent(model.Amount.ToString(System.Globalization.CultureInfo.InvariantCulture)), "amount");
                multipart.Add(new StringContent(model.PaymentDescription ?? ""), "paymentDescription");

                if (!string.IsNullOrWhiteSpace(model.BankAccountNumber))
                    multipart.Add(new StringContent(model.BankAccountNumber), "bankAccountNumber");

                if (model.ReceiptImage != null && model.ReceiptImage.Length > 0)
                {
                    var fileContent = new StreamContent(model.ReceiptImage.OpenReadStream());
                    fileContent.Headers.ContentType =
                        new System.Net.Http.Headers.MediaTypeHeaderValue(model.ReceiptImage.ContentType ?? "application/octet-stream");

                    multipart.Add(fileContent, "receiptImage", model.ReceiptImage.FileName);
                }

                var res = _client.PostAsync(url, multipart).Result;

                if (res.IsSuccessStatusCode)
                    return new BaseResult { Status = true, Message = "درخواست شارژ با موفقیت ثبت شد." };

                return new BaseResult { Status = false, Message = res.Content.ReadAsStringAsync().Result };
            }
            catch (Exception ex)
            {
                // اگر AggregateException بود، پیام داخلی رو هم بده
                var inner = ex.InnerException?.Message;
                return new BaseResult { Status = false, Message = inner ?? ex.Message };
            }
        }

        public List<SenderNumberOptionDTO> GetSenderNumberOptions()
        {
            try
            {
                SetAuth();

                var url = $"{_baseUrl}/api/admin/sender-numbers/search";

                var body = new
                {
                    page = 1,
                    pageSize = 200,
                    filters = new List<object>(),
                    sortBy = "fixedPrefix",   // حروف درست
                    sortDescending = false
                };

                var json = JsonSerializer.Serialize(body, JsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var res = _client.PostAsync(url, content).Result;
                var jsonResult = res.Content.ReadAsStringAsync().Result;

                if (!res.IsSuccessStatusCode || string.IsNullOrWhiteSpace(jsonResult))
                    return new List<SenderNumberOptionDTO>();

                using var doc = JsonDocument.Parse(jsonResult);
                var root = doc.RootElement;

                if (!root.TryGetProperty("data", out var dataEl) || dataEl.ValueKind != JsonValueKind.Array)
                    return new List<SenderNumberOptionDTO>();

                var list = new List<SenderNumberOptionDTO>();

                foreach (var item in dataEl.EnumerateArray())
                {
                    // sender-numbers/search => کلید درست: id
                    var id = item.TryGetProperty("id", out var idEl) ? idEl.ToString() : null;
                    if (string.IsNullOrWhiteSpace(id))
                        continue;

                    var fullNumber = item.TryGetProperty("fullNumber", out var fn) ? fn.GetString() : null;
                    var fixedPrefix = item.TryGetProperty("fixedPrefix", out var fp) ? fp.GetString() : null;

                    var display = !string.IsNullOrWhiteSpace(fullNumber)
                        ? fullNumber
                        : (!string.IsNullOrWhiteSpace(fixedPrefix) ? fixedPrefix : id);

                    list.Add(new SenderNumberOptionDTO
                    {
                        Id = id,
                        Display = display
                    });
                }

                return list
                    .GroupBy(x => x.Id)
                    .Select(g => g.First())
                    .ToList();
            }
            catch
            {
                return new List<SenderNumberOptionDTO>();
            }
        }

    }
}
