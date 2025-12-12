using DTO.Base;
using DTO.DataTable;
using DTO.Project.SenderChargeRequest;
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

namespace BLL.Project.SenderChargeRequest
{
    public class ChargeRequestManager : IChargeRequestManager
    {
        private readonly IHttpContextAccessor _httpContext;
        private readonly HttpClient _client;
        private readonly string _baseUrl;

        public ChargeRequestManager(IHttpContextAccessor accessor, IConfiguration config)
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
        public List<ChargeRequestListDTO> GetAll()
        {
            try
            {
                SetAuth();

                var url = $"{_baseUrl}/api/admin/sender-charge-requests/search";

                var body = new
                {
                    page = 1,
                    pageSize = 1000,
                    filters = new List<object>(),
                    sortBy = "createdOn",
                    sortDescending = true
                };

                var jsonBody = JsonSerializer.Serialize(body, JsonOptions);
                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                var res = _client.PostAsync(url, content).Result;
                var jsonResult = res.Content.ReadAsStringAsync().Result;

                if (!res.IsSuccessStatusCode || string.IsNullOrWhiteSpace(jsonResult))
                    return new List<ChargeRequestListDTO>();

                var apiResponse =
                    JsonSerializer.Deserialize<ApiResponsePagedDTO<ChargeRequestListDTO>>(jsonResult, JsonOptions);

                var data = apiResponse?.Data ?? new List<ChargeRequestListDTO>();

                // اینجا enrich می‌کنیم
                EnrichChargeRequests(data);

                return data;
            }
            catch
            {
                return new List<ChargeRequestListDTO>();
            }
        }

        /* ----------------------------------------------
         *  DataTable
         * ----------------------------------------------*/
        public DataTableResponseDTO<ChargeRequestListDTO> GetDataTableDTO(DataTableSearchDTO search)
        {
            try
            {
                SetAuth();

                var url = $"{_baseUrl}/api/admin/sender-charge-requests/search";

                var body = new
                {
                    page = (search.start / search.length) + 1,
                    pageSize = search.length,
                    filters = new List<object>(),
                    sortBy = string.IsNullOrWhiteSpace(search.sortColumnName) ? "createdOn" : search.sortColumnName,
                    sortDescending = search.sortDirection == "desc"
                };

                var jsonBody = JsonSerializer.Serialize(body, JsonOptions);
                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                var res = _client.PostAsync(url, content).Result;
                var jsonResult = res.Content.ReadAsStringAsync().Result;

                if (!res.IsSuccessStatusCode || string.IsNullOrWhiteSpace(jsonResult))
                {
                    return new DataTableResponseDTO<ChargeRequestListDTO>
                    {
                        draw = search.draw,
                        recordsTotal = 0,
                        recordsFiltered = 0,
                        data = new List<ChargeRequestListDTO>(),
                        AdditionalData = jsonResult
                    };
                }

                var apiResponse =
                    JsonSerializer.Deserialize<ApiResponsePagedDTO<ChargeRequestListDTO>>(jsonResult, JsonOptions);

                var total = apiResponse?.Meta?.TotalCount ?? 0;
                var data = apiResponse?.Data ?? new List<ChargeRequestListDTO>();


                // enrich لیست
                EnrichChargeRequests(data);

                return new DataTableResponseDTO<ChargeRequestListDTO>
                {
                    draw = search.draw,
                    recordsTotal = total,
                    recordsFiltered = total,
                    data = data
                };
            }
            catch (Exception ex)
            {
                return new DataTableResponseDTO<ChargeRequestListDTO>
                {
                    draw = search.draw,
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = new List<ChargeRequestListDTO>(),
                    AdditionalData = ex.Message
                };
            }
        }

        /* ----------------------------------------------
         *  Approve
         * ----------------------------------------------*/
        public BaseResult Approve(string id, string note)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                    return new BaseResult { Status = false, Message = "شناسه درخواست نامعتبر است." };

                SetAuth();

                var url = $"{_baseUrl}/api/admin/sender-charge-requests/{id}/approve";

                var body = new
                {
                    note = string.IsNullOrWhiteSpace(note) ? null : note
                };

                var jsonBody = JsonSerializer.Serialize(body, JsonOptions);
                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                var res = _client.PutAsync(url, content).Result;
                var msg = res.Content.ReadAsStringAsync().Result;

                if (res.IsSuccessStatusCode)
                    return new BaseResult { Status = true, Message = "درخواست با موفقیت تایید شد." };

                return new BaseResult
                {
                    Status = false,
                    Message = string.IsNullOrWhiteSpace(msg) ? "خطا در تایید درخواست شارژ." : msg
                };
            }
            catch (Exception ex)
            {
                return new BaseResult { Status = false, Message = ex.Message };
            }
        }

        /* ----------------------------------------------
         *  Reject
         * ----------------------------------------------*/
        public BaseResult Reject(string id, string note)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                    return new BaseResult { Status = false, Message = "شناسه درخواست نامعتبر است." };

                SetAuth();

                var url = $"{_baseUrl}/api/admin/sender-charge-requests/{id}/reject";

                var body = new
                {
                    note = string.IsNullOrWhiteSpace(note) ? null : note
                };

                var jsonBody = JsonSerializer.Serialize(body, JsonOptions);
                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                var res = _client.PutAsync(url, content).Result;
                var msg = res.Content.ReadAsStringAsync().Result;

                if (res.IsSuccessStatusCode)
                    return new BaseResult { Status = true, Message = "درخواست با موفقیت رد شد." };

                return new BaseResult
                {
                    Status = false,
                    Message = string.IsNullOrWhiteSpace(msg) ? "خطا در رد درخواست شارژ." : msg
                };
            }
            catch (Exception ex)
            {
                return new BaseResult { Status = false, Message = ex.Message };
            }
        }


        private Dictionary<string, SenderNumberInfoDTO> GetSenderNumberMap(IEnumerable<string> ids)
        {
            var result = new Dictionary<string, SenderNumberInfoDTO>(StringComparer.OrdinalIgnoreCase);

            var idList = ids
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (idList.Count == 0)
                return result;

            SetAuth();

            foreach (var id in idList)
            {
                try
                {
                    var url = $"{_baseUrl}/api/admin/sender-numbers/{id}";

                    var res = _client.GetAsync(url).Result;
                    if (!res.IsSuccessStatusCode)
                        continue;

                    var json = res.Content.ReadAsStringAsync().Result;
                    if (string.IsNullOrWhiteSpace(json))
                        continue;

                    var apiResponse =
                        JsonSerializer.Deserialize<ApiResponseSingleDTO<SenderNumberInfoDTO>>(
                            json, JsonOptions);

                    var sn = apiResponse?.Data;
                    if (sn == null || string.IsNullOrWhiteSpace(sn.Id))
                        continue;

                    if (!result.ContainsKey(sn.Id))
                    {
                        result[sn.Id] = sn;
                    }
                }
                catch
                {
                    // اگر یکی خراب شد، کل لیست رو نمی‌ترکونیم
                    continue;
                }
            }

            return result;
        }

        private void EnrichChargeRequests(List<ChargeRequestListDTO> items)
        {
            if (items == null || items.Count == 0)
                return;

            // ===== ۱) پر کردن FullNumber سرشماره‌ها =====
            var senderIdList = items
                .Select(x => x.SenderNumberId)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToList();

            var senderMap = GetSenderNumberMap(senderIdList);

            foreach (var item in items)
            {
                if (!string.IsNullOrWhiteSpace(item.SenderNumberId) &&
                    senderMap.TryGetValue(item.SenderNumberId, out var sn))
                {
                    item.SenderNumberFullNumber = sn.FullNumber;
                }
            }

            // ===== ۲) جا برای کیف پول و کاربر =====
            // اینجا اگر سرویس Wallet و User را داشتی، دقیقاً مثل بالا
            // می‌توانی متدهای مشابه GetWalletMap / GetUserMap بنویسی و
            // item.WalletTitle / item.RequestedByUserFullName را پر کنی.
        }


    }
}
