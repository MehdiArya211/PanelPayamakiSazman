using DTO.DataTable;
using DTO.Project.ConsumerSenderNumbers;
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

namespace BLL.Project.ConsumerSenderNumbers
{
    public class ConsumerSenderNumberManager : IConsumerSenderNumberManager
    {
        private readonly IHttpContextAccessor _httpContext;
        private readonly HttpClient _client;
        private readonly string _baseUrl;

        public ConsumerSenderNumberManager(IHttpContextAccessor accessor, IConfiguration config)
        {
            _httpContext = accessor;
            _client = new HttpClient();
            //_baseUrl = config["ApiBaseUrl"] ?? "http://87.107.111.44:8010";
            _baseUrl = "http://87.107.111.44:8010";
        }

        private void SetAuth()
        {
            var token = _httpContext.HttpContext.Session.GetString("AdminToken");
            // اگر تو consumer توکن جدا داری (ConsumerToken)، همینجا عوض کن.

            _client.DefaultRequestHeaders.Authorization = null;

            if (!string.IsNullOrWhiteSpace(token))
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        private JsonSerializerOptions JsonOptions => new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        public DataTableResponseDTO<UserSenderNumberAccessListDTO> GetDataTableDTO(DataTableSearchDTO search)
        {
            SetAuth();

            var url = $"{_baseUrl}/api/consumer/sender-numbers/search";

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
                return new DataTableResponseDTO<UserSenderNumberAccessListDTO>
                {
                    draw = search.draw,
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = new List<UserSenderNumberAccessListDTO>(),
                    AdditionalData = jsonResult
                };
            }

            var apiResponse = JsonSerializer.Deserialize<ApiResponsePagedDTO<UserSenderNumberAccessListDTO>>(jsonResult, JsonOptions);

            var total = apiResponse?.Meta?.TotalCount ?? 0;
            var data = apiResponse?.Data ?? new List<UserSenderNumberAccessListDTO>();

            return new DataTableResponseDTO<UserSenderNumberAccessListDTO>
            {
                draw = search.draw,
                recordsTotal = total,
                recordsFiltered = total,
                data = data
            };
        }

        public List<SenderNumberOptionDTO> GetOptions()
        {
            try
            {
                SetAuth();

                var url = $"{_baseUrl}/api/consumer/sender-numbers/search";

                var body = new
                {
                    page = 1,
                    pageSize = 500,
                    filters = new List<object>(),
                    sortBy = "fixedprefix",
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
                    // اینجا ممکنه id = senderNumberId باشه یا accessId. طبق خروجی واقعی تنظیمش کن.
                    var id = item.TryGetProperty("senderNumberId", out var sid) ? sid.ToString()
                           : item.TryGetProperty("id", out var idEl) ? idEl.ToString()
                           : null;

                    var fullNumber = item.TryGetProperty("fullNumber", out var fn) ? fn.GetString()
                                  : item.TryGetProperty("senderNumber", out var sn) ? sn.GetString()
                                  : null;

                    if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(fullNumber))
                        continue;

                    list.Add(new SenderNumberOptionDTO { Id = id, Display = fullNumber });
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
