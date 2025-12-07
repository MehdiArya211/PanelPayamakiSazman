using DTO.Base;
using DTO.DataTable;
using DTO.Project.SecurityQuestion;
using DTO.WebApi;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BLL.Project.SecurityQuestion
{
    public class SecurityQuestionManager: ISecurityQuestionManager
    {
        private readonly IHttpContextAccessor _httpContext;
        private readonly HttpClient _client;

        public SecurityQuestionManager(IHttpContextAccessor accessor)
        {
            _httpContext = accessor;
            _client = new HttpClient();
        }

        /* ----------------------------------------------
         *  هدر احراز هویت
         * ----------------------------------------------*/
        private void SetAuth()
        {
            var token = _httpContext.HttpContext.Session.GetString("AdminToken");

            _client.DefaultRequestHeaders.Authorization = null;

            if (!string.IsNullOrWhiteSpace(token))
                _client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
        }

        /* ----------------------------------------------
         *  دریافت همه سوالات
         * ----------------------------------------------*/
        public List<SecurityQuestionListDTO> GetAll()
        {
            SetAuth();

            // طبق نمونه خودت: security-questions/null
            var url = "http://87.107.111.44:8010/api/admin/security-questions/null";

            var res = _client.GetAsync(url).Result;

            var json = res.Content.ReadAsStringAsync().Result;

            return JsonSerializer.Deserialize<List<SecurityQuestionListDTO>>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                ?? new List<SecurityQuestionListDTO>();
        }

        /* ----------------------------------------------
         *  ایجاد سوال
         * ----------------------------------------------*/
        public BaseResult Create(SecurityQuestionCreateDTO model)
        {
            try
            {
                SetAuth();

                var url = "http://87.107.111.44:8010/api/admin/security-questions";

                var body = new { text = model.Text }; // 👈 کاملاً مطابق API

                var json = JsonSerializer.Serialize(body);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var res = _client.PostAsync(url, content).Result;

                if (res.IsSuccessStatusCode)
                    return new BaseResult(true, "سوال امنیتی با موفقیت ثبت شد.");

                return new BaseResult(false, res.Content.ReadAsStringAsync().Result);
            }
            catch (Exception ex)
            {
                return new BaseResult(false, ex.Message);
            }
        }


        /* ----------------------------------------------
         *  خروجی برای DataTable
         * ----------------------------------------------*/
        public DataTableResponseDTO<SecurityQuestionListDTO> GetDataTableDTO(DataTableSearchDTO search)
        {
            SetAuth();

            var url = "http://87.107.111.44:8010/api/admin/security-questions/search";

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
            var jsonResult = res.Content.ReadAsStringAsync().Result;

            if (!res.IsSuccessStatusCode)
            {
                return new DataTableResponseDTO<SecurityQuestionListDTO>
                {
                    draw = search.draw,
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = new List<SecurityQuestionListDTO>(),
                    AdditionalData = jsonResult
                };
            }

            var apiResponse = JsonSerializer.Deserialize<ApiResponsePagedDTO<SecurityQuestionListDTO>>(
                jsonResult,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return new DataTableResponseDTO<SecurityQuestionListDTO>
            {
                draw = search.draw,
                recordsTotal = apiResponse.Meta.TotalCount,
                recordsFiltered = apiResponse.Meta.TotalCount,
                data = apiResponse.Data
            };
        }

        /* ----------------------------------------------
         *  دریافت یک آیتم برای ویرایش
         * ----------------------------------------------*/
        public SecurityQuestionEditDTO GetById(string id)
        {
            try
            {
                SetAuth();

                var url = $"http://87.107.111.44:8010/api/admin/security-questions/{id}";

                var res = _client.GetAsync(url).Result;

                if (!res.IsSuccessStatusCode)
                    return null;

                var json = res.Content.ReadAsStringAsync().Result;

                var apiResponse =
                    JsonSerializer.Deserialize<ApiResponseSingleDTO<SecurityQuestionEditDTO>>(
                        json,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return apiResponse?.Data;
            }
            catch
            {
                return null;
            }
        }

        /* ----------------------------------------------
         *  ویرایش سوال
         * ----------------------------------------------*/
        public BaseResult Update(SecurityQuestionEditDTO model)
        {
            try
            {
                SetAuth();

                var url = $"http://87.107.111.44:8010/api/admin/security-questions/{model.Id}";

                var json = JsonSerializer.Serialize(model);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var res = _client.PutAsync(url, content).Result;

                if (res.IsSuccessStatusCode)
                    return new BaseResult(true, "سوال امنیتی با موفقیت ویرایش شد.");

                return new BaseResult(false, res.Content.ReadAsStringAsync().Result);
            }
            catch (Exception ex)
            {
                return new BaseResult(false, ex.Message);
            }
        }

        /* ----------------------------------------------
         *  حذف سوال
         * ----------------------------------------------*/
        public BaseResult Delete(string id)
        {
            try
            {
                SetAuth();

                var url = $"http://87.107.111.44:8010/api/admin/security-questions/{id}";

                var res = _client.DeleteAsync(url).Result;

                if (res.IsSuccessStatusCode)
                    return new BaseResult(true, "سوال امنیتی با موفقیت حذف شد.");

                return new BaseResult(false, res.Content.ReadAsStringAsync().Result);
            }
            catch (Exception ex)
            {
                return new BaseResult(false, ex.Message);
            }
        }
    }
}
