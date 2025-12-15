using DTO.Base;
using DTO.DataTable;
using DTO.Project.SmsTariffs;
using DTO.Project.WebApi;
using DTO.WebApi;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace BLL.Project.SmsTariff
{
    public class SmsTariffManager : ISmsTariffManager
    {
        private readonly IHttpContextAccessor _httpContext;
        private readonly HttpClient _client;
        private readonly string _baseUrl;

        public SmsTariffManager(IHttpContextAccessor accessor, IConfiguration config)
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

        /* ---------------------------
         * DataTable
         * --------------------------- */
        public DataTableResponseDTO<SmsTariffListDTO> GetDataTableDTO(DataTableSearchDTO search)
        {
            SetAuth();

            var url = $"{_baseUrl}/sms-tariffs/search";

            var body = new
            {
                page = (search.start / search.length) + 1,
                pageSize = search.length,
                filters = new List<object>(),
                sortBy = search.sortColumnName,
                sortDescending = search.sortDirection == "desc"
            };

            var content = new StringContent(
                JsonSerializer.Serialize(body),
                Encoding.UTF8,
                "application/json");

            var res = _client.PostAsync(url, content).Result;
            var jsonResult = res.Content.ReadAsStringAsync().Result;

            if (!res.IsSuccessStatusCode)
            {
                return new DataTableResponseDTO<SmsTariffListDTO>
                {
                    draw = search.draw,
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = new List<SmsTariffListDTO>(),
                    AdditionalData = jsonResult
                };
            }

            var api =
                JsonSerializer.Deserialize<ApiResponsePagedDTO<SmsTariffListDTO>>(
                    jsonResult,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return new DataTableResponseDTO<SmsTariffListDTO>
            {
                draw = search.draw,
                recordsTotal = api.Meta.TotalCount,
                recordsFiltered = api.Meta.TotalCount,
                data = api.Data
            };
        }

        /* ---------------------------
         * Get By Id
         * --------------------------- */
        public SmsTariffEditDTO GetById(string id)
        {
            SetAuth();

            var url = $"{_baseUrl}/sms-tariffs/{id}";
            var res = _client.GetAsync(url).Result;

            if (!res.IsSuccessStatusCode)
                return null;

            var json = res.Content.ReadAsStringAsync().Result;

            var api =
                JsonSerializer.Deserialize<ApiResponseSingleDTO<SmsTariffEditDTO>>(
                    json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return api?.Data;
        }

        /* ---------------------------
         * Create
         * --------------------------- */
        public BaseResult Create(SmsTariffCreateDTO model)
        {
            try
            {
                SetAuth();

                var url = $"{_baseUrl}/sms-tariffs";

                var body = new
                {
                    Operator = model.Operator,
                    PersianPricePerSegment = model.PersianPricePerSegment,
                    EnglishPricePerSegment = model.EnglishPricePerSegment
                };

                var content = new StringContent(
                    JsonSerializer.Serialize(body),
                    Encoding.UTF8,
                    "application/json");

                var res = _client.PostAsync(url, content).Result;

                if (res.IsSuccessStatusCode)
                    return new BaseResult(true, "تعرفه پیامک با موفقیت ثبت شد.");

                return new BaseResult(false, res.Content.ReadAsStringAsync().Result);
            }
            catch (Exception ex)
            {
                return new BaseResult(false, ex.Message);
            }
        }

        /* ---------------------------
         * Update
         * --------------------------- */
        public BaseResult Update0(SmsTariffEditDTO model)
        {
            try
            {
                SetAuth();

                var url = $"{_baseUrl}/sms-tariffs/{model.Id}";

                var body = new
                {
                    Operator = model.Operator,
                    PersianPricePerSegment = model.PersianPricePerSegment,
                    EnglishPricePerSegment = model.EnglishPricePerSegment
                };

                var content = new StringContent(
                    JsonSerializer.Serialize(body),
                    Encoding.UTF8,
                    "application/json");

                var res = _client.PutAsync(url, content).Result;

                if (res.IsSuccessStatusCode)
                    return new BaseResult(true, "تعرفه پیامک با موفقیت ویرایش شد.");

                return new BaseResult(false, res.Content.ReadAsStringAsync().Result);
            }
            catch (Exception ex)
            {
                return new BaseResult(false, ex.Message);
            }
        }

        public BaseResult Update(SmsTariffEditDTO model)
        {
            try
            {
                SetAuth();

                var url = $"{_baseUrl}/sms-tariffs/{model.Id}";

                var body = new Dictionary<string, object>
                {
                    ["operator"] = model.Operator.ToString(),
                    ["persianPricePerSegment"] = model.PersianPricePerSegment,
                    ["englishPricePerSegment"] = model.EnglishPricePerSegment
                };

                var json = JsonSerializer.Serialize(body);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var res = _client.PutAsync(url, content).Result;

                if (res.IsSuccessStatusCode)
                    return new BaseResult(true, "تعرفه پیامک با موفقیت ویرایش شد.");

                var error = res.Content.ReadAsStringAsync().Result;
                return new BaseResult(false, error);
            }
            catch (Exception ex)
            {
                return new BaseResult(false, ex.Message);
            }
        }


        /* ---------------------------
         * Delete
         * --------------------------- */
        public BaseResult Delete(string id)
        {
            try
            {
                SetAuth();

                var url = $"{_baseUrl}/sms-tariffs/{id}";
                var res = _client.DeleteAsync(url).Result;

                if (res.IsSuccessStatusCode)
                    return new BaseResult(true, "تعرفه پیامک حذف شد.");

                return new BaseResult(false, res.Content.ReadAsStringAsync().Result);
            }
            catch (Exception ex)
            {
                return new BaseResult(false, ex.Message);
            }
        }
    }
}
