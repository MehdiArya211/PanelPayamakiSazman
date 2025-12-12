using DTO.Base;
using DTO.DataTable;
using DTO.Project.SenderWallet;
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

namespace BLL.Project.SenderWallet
{
    public class SenderWalletManager : ISenderWalletManager
    {
        private readonly IHttpContextAccessor _httpContext;
        private readonly HttpClient _client;
        private readonly string _baseUrl;

        public SenderWalletManager(IHttpContextAccessor accessor, IConfiguration config)
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
         *  GetWallet
         * ----------------------------------------------*/
        public SenderWalletDTO GetWallet(string senderNumberId)
        {
            if (string.IsNullOrWhiteSpace(senderNumberId))
                return null;

            try
            {
                SetAuth();

                var url = $"{_baseUrl}/api/admin/sender-wallets/{senderNumberId}";

                var res = _client.GetAsync(url).Result;
                var jsonResult = res.Content.ReadAsStringAsync().Result;

                if (!res.IsSuccessStatusCode || string.IsNullOrWhiteSpace(jsonResult))
                    return null;

                var apiResponse =
                    JsonSerializer.Deserialize<ApiResponseListDTO<SenderWalletDTO>>(
                        jsonResult, JsonOptions);

                var data = apiResponse?.Data;
                if (data == null )
                    return null;

                // فرض: برای هر سرشماره یک کیف پول داریم
                return data;
            }
            catch
            {
                return null;
            }
        }

        /* ----------------------------------------------
         *  GetTransactions (DataTable)
         * ----------------------------------------------*/
        public DataTableResponseDTO<WalletTransactionDTO> GetTransactions(
            string senderNumberId,
            DataTableSearchDTO search)
        {
            if (string.IsNullOrWhiteSpace(senderNumberId))
            {
                return new DataTableResponseDTO<WalletTransactionDTO>
                {
                    draw = search.draw,
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = new List<WalletTransactionDTO>(),
                    AdditionalData = "senderNumberId is empty"
                };
            }

            try
            {
                SetAuth();

                var url = $"{_baseUrl}/api/admin/sender-wallets/{senderNumberId}/transactions/search";

                var filters = new List<object>();

                if (!string.IsNullOrWhiteSpace(search.searchValue))
                {
                    // مثال: جستجو روی description
                    filters.Add(new
                    {
                        field = "description",
                        @operator = "Contains",
                        value = search.searchValue
                    });
                }

                var body = new
                {
                    page = (search.start / search.length) + 1,
                    pageSize = search.length,
                    filters = filters,
                    sortBy = string.IsNullOrWhiteSpace(search.sortColumnName) ? "createdOn" : search.sortColumnName,
                    sortDescending = search.sortDirection == "desc"
                };

                var jsonBody = JsonSerializer.Serialize(body, JsonOptions);
                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                var res = _client.PostAsync(url, content).Result;
                var jsonResult = res.Content.ReadAsStringAsync().Result;

                if (!res.IsSuccessStatusCode || string.IsNullOrWhiteSpace(jsonResult))
                {
                    return new DataTableResponseDTO<WalletTransactionDTO>
                    {
                        draw = search.draw,
                        recordsTotal = 0,
                        recordsFiltered = 0,
                        data = new List<WalletTransactionDTO>(),
                        AdditionalData = jsonResult
                    };
                }

                var apiResponse =
                    JsonSerializer.Deserialize<ApiResponsePagedDTO<WalletTransactionDTO>>(
                        jsonResult, JsonOptions);

                var total = apiResponse?.Meta?.TotalCount ?? 0;
                var data = apiResponse?.Data ?? new List<WalletTransactionDTO>();

                return new DataTableResponseDTO<WalletTransactionDTO>
                {
                    draw = search.draw,
                    recordsTotal = total,
                    recordsFiltered = total,
                    data = data
                };
            }
            catch (Exception ex)
            {
                return new DataTableResponseDTO<WalletTransactionDTO>
                {
                    draw = search.draw,
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = new List<WalletTransactionDTO>(),
                    AdditionalData = ex.Message
                };
            }
        }

        /* ----------------------------------------------
         *  Charge
         * ----------------------------------------------*/
        public BaseResult Charge(SenderWalletChargeDTO model)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(model.SenderNumberId))
                    return new BaseResult { Status = false, Message = "سرشماره نامعتبر است." };

                SetAuth();

                var url = $"{_baseUrl}/api/admin/sender-wallets/{model.SenderNumberId}/charge";

                var body = new
                {
                    amount = model.Amount,
                    description = model.Description
                };

                var jsonBody = JsonSerializer.Serialize(body, JsonOptions);
                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                var res = _client.PostAsync(url, content).Result;
                var msg = res.Content.ReadAsStringAsync().Result;

                if (res.IsSuccessStatusCode)
                    return new BaseResult { Status = true, Message = "کیف پول با موفقیت شارژ شد." };

                return new BaseResult
                {
                    Status = false,
                    Message = string.IsNullOrWhiteSpace(msg) ? "خطا در شارژ کیف پول." : msg
                };
            }
            catch (Exception ex)
            {
                return new BaseResult { Status = false, Message = ex.Message };
            }
        }

        /* ----------------------------------------------
         *  Transfer
         * ----------------------------------------------*/
        public BaseResult Transfer(SenderWalletTransferDTO model)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(model.FromSenderNumberId) ||
                    string.IsNullOrWhiteSpace(model.ToSenderNumberId))
                {
                    return new BaseResult { Status = false, Message = "سرشماره مبدأ یا مقصد نامعتبر است." };
                }

                if (model.FromSenderNumberId == model.ToSenderNumberId)
                {
                    return new BaseResult { Status = false, Message = "سرشماره مبدأ و مقصد نمی‌تواند یکسان باشد." };
                }

                SetAuth();

                var url = $"{_baseUrl}/api/admin/sender-wallets/transfer";

                var body = new
                {
                    fromSenderNumberId = model.FromSenderNumberId,
                    toSenderNumberId = model.ToSenderNumberId,
                    amount = model.Amount,
                    description = model.Description
                };

                var jsonBody = JsonSerializer.Serialize(body, JsonOptions);
                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                var res = _client.PostAsync(url, content).Result;
                var msg = res.Content.ReadAsStringAsync().Result;

                if (res.IsSuccessStatusCode)
                    return new BaseResult { Status = true, Message = "انتقال بین کیف پول‌ها با موفقیت انجام شد." };

                return new BaseResult
                {
                    Status = false,
                    Message = string.IsNullOrWhiteSpace(msg) ? "خطا در انتقال موجودی." : msg
                };
            }
            catch (Exception ex)
            {
                return new BaseResult { Status = false, Message = ex.Message };
            }
        }

        /* ----------------------------------------------
         *  SenderNumber Lookup (برای دراپ‌داون‌ها)
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
    }
}
