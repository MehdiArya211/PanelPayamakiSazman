using DTO.Base;
using DTO.DataTable;
using DTO.Project.SenderNumberOrganizationLevel;
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

namespace BLL.Project.SenderNumberOrganizationLevel
{
    public class SenderNumberOrganizationLevelManager : ISenderNumberOrganizationLevelManager
    {
        private readonly IHttpContextAccessor _httpContext;
        private readonly HttpClient _client;
        private readonly string _baseUrl;

        public SenderNumberOrganizationLevelManager(IHttpContextAccessor accessor, IConfiguration config)
        {
            _httpContext = accessor;
            _client = new HttpClient();
            //_baseUrl = config["ApiBaseUrl"] ?? "http://87.107.111.44:8010";
            _baseUrl = "http://87.107.111.44:8010";
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
         *  GetAll
         * ----------------------------------------------*/
        public List<SenderNumberOrganizationLevelListDTO> GetAll()
        {
            try
            {
                SetAuth();

                var url = $"{_baseUrl}/api/admin/sender-number-organization-levels/search";

                var body = new
                {
                    page = 1,
                    pageSize = 1000,
                    filters = new List<object>(),
                    sortBy = "code",
                    sortDescending = false
                };

                var json = JsonSerializer.Serialize(body, JsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var res = _client.PostAsync(url, content).Result;
                var jsonResult = res.Content.ReadAsStringAsync().Result;

                if (!res.IsSuccessStatusCode)
                    return new List<SenderNumberOrganizationLevelListDTO>();

                var apiResponse =
                    JsonSerializer.Deserialize<ApiResponsePagedDTO<SenderNumberOrganizationLevelListDTO>>(
                        jsonResult, JsonOptions);

                return apiResponse?.Data ?? new List<SenderNumberOrganizationLevelListDTO>();
            }
            catch
            {
                return new List<SenderNumberOrganizationLevelListDTO>();
            }
        }

        /* ----------------------------------------------
         *  DataTable
         * ----------------------------------------------*/
        public DataTableResponseDTO<SenderNumberOrganizationLevelListDTO> GetDataTableDTO(DataTableSearchDTO search)
        {
            SetAuth();

            var url = $"{_baseUrl}/api/admin/sender-number-organization-levels/search";

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
                return new DataTableResponseDTO<SenderNumberOrganizationLevelListDTO>
                {
                    draw = search.draw,
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = new List<SenderNumberOrganizationLevelListDTO>(),
                    AdditionalData = jsonResult
                };
            }

            var apiResponse =
                JsonSerializer.Deserialize<ApiResponsePagedDTO<SenderNumberOrganizationLevelListDTO>>(
                    jsonResult, JsonOptions);

            var total = apiResponse?.Meta?.TotalCount ?? 0;
            var data = apiResponse?.Data ?? new List<SenderNumberOrganizationLevelListDTO>();

            return new DataTableResponseDTO<SenderNumberOrganizationLevelListDTO>
            {
                draw = search.draw,
                recordsTotal = total,
                recordsFiltered = total,
                data = data
            };
        }

        /* ----------------------------------------------
         *  Create
         * ----------------------------------------------*/
        public BaseResult Create(SenderNumberOrganizationLevelCreateDTO model)
        {
            try
            {
                SetAuth();

                var url = $"{_baseUrl}/api/admin/sender-number-organization-levels";

                var body = new
                {
                    code = model.code,
                    title = model.title,
                    description = model.description
                };

                var json = JsonSerializer.Serialize(body, JsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var res = _client.PostAsync(url, content).Result;

                if (res.IsSuccessStatusCode)
                    return new BaseResult { Status = true, Message = "با موفقیت ثبت شد." };

                var msg = res.Content.ReadAsStringAsync().Result;
                return new BaseResult { Status = false, Message = msg };
            }
            catch (Exception ex)
            {
                return new BaseResult { Status = false, Message = ex.Message };
            }
        }

        /* ----------------------------------------------
         *  GetById
         * ----------------------------------------------*/
        public SenderNumberOrganizationLevelEditDTO GetById(string id)
        {
            try
            {
                SetAuth();

                var url = $"{_baseUrl}/api/admin/sender-number-organization-levels/{id}";

                var res = _client.GetAsync(url).Result;
                if (!res.IsSuccessStatusCode)
                    return null;

                var jsonResult = res.Content.ReadAsStringAsync().Result;

                var apiResponse =
                    JsonSerializer.Deserialize<ApiResponseSingleDTO<SenderNumberOrganizationLevelEditDTO>>(
                        jsonResult, JsonOptions);

                return apiResponse?.Data;
            }
            catch
            {
                return null;
            }
        }

        /* ----------------------------------------------
         *  Update
         * ----------------------------------------------*/
        public BaseResult Update(SenderNumberOrganizationLevelEditDTO model)
        {
            try
            {
                SetAuth();

                var url = $"{_baseUrl}/api/admin/sender-number-organization-levels/{model.Id}";

                var body = new
                {
                    code = model.Code,
                    title = model.Title,
                    description = model.Description
                };

                var json = JsonSerializer.Serialize(body, JsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var res = _client.PutAsync(url, content).Result;

                if (res.IsSuccessStatusCode)
                    return new BaseResult { Status = true, Message = "با موفقیت ویرایش شد." };

                var msg = res.Content.ReadAsStringAsync().Result;
                return new BaseResult { Status = false, Message = msg };
            }
            catch (Exception ex)
            {
                return new BaseResult { Status = false, Message = ex.Message };
            }
        }

        /* ----------------------------------------------
         *  Delete
         * ----------------------------------------------*/
        public BaseResult Delete(string id)
        {
            try
            {
                SetAuth();

                var url = $"{_baseUrl}/api/admin/sender-number-organization-levels/{id}";

                var res = _client.DeleteAsync(url).Result;

                if (res.IsSuccessStatusCode)
                    return new BaseResult { Status = true, Message = "با موفقیت حذف شد." };

                var msg = res.Content.ReadAsStringAsync().Result;
                return new BaseResult { Status = false, Message = msg };
            }
            catch (Exception ex)
            {
                return new BaseResult { Status = false, Message = ex.Message };
            }
        }
    }
}
