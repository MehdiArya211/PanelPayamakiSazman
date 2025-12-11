using DTO.Base;
using DTO.DataTable;
using DTO.Project.SenderNumber;
using DTO.Project.SenderNumberOrganizationLevel;
using DTO.Project.SenderNumberSpeciality;
using DTO.Project.SenderNumberSubAreaList;
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

namespace BLL.Project.SenderNumber
{
    public class SenderNumberManager : ISenderNumberManager
    {
        private readonly IHttpContextAccessor _httpContext;
        private readonly HttpClient _client;
        private readonly string _baseUrl;

        public SenderNumberManager(IHttpContextAccessor accessor, IConfiguration config)
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
         *  سرشماره‌ها: GetAll
         * ----------------------------------------------*/
        public List<SenderNumberListDTO> GetAll()
        {
            try
            {
                SetAuth();

                var url = $"{_baseUrl}/api/admin/sender-numbers/search";

                var body = new
                {
                    page = 1,
                    pageSize = 1000,
                    filters = new List<object>(),
                    sortBy = "fixedPrefix",
                    sortDescending = false
                };

                var json = JsonSerializer.Serialize(body, JsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var res = _client.PostAsync(url, content).Result;
                var jsonResult = res.Content.ReadAsStringAsync().Result;

                if (!res.IsSuccessStatusCode || string.IsNullOrWhiteSpace(jsonResult))
                    return new List<SenderNumberListDTO>();

                var apiResponse =
                    JsonSerializer.Deserialize<ApiResponsePagedDTO<SenderNumberListDTO>>(jsonResult, JsonOptions);

                return apiResponse?.Data ?? new List<SenderNumberListDTO>();
            }
            catch
            {
                return new List<SenderNumberListDTO>();
            }
        }

        /* ----------------------------------------------
         *  سرشماره‌ها: DataTable
         * ----------------------------------------------*/
        public DataTableResponseDTO<SenderNumberListDTO> GetDataTableDTO(DataTableSearchDTO search)
        {
            try
            {
                SetAuth();

                var url = $"{_baseUrl}/api/admin/sender-numbers/search";

                var body = new
                {
                    page = (search.start / search.length) + 1,
                    pageSize = search.length,
                    filters = new List<object>(),
                    sortBy = string.IsNullOrWhiteSpace(search.sortColumnName) ? "fixedPrefix" : search.sortColumnName,
                    sortDescending = search.sortDirection == "desc"
                };

                var json = JsonSerializer.Serialize(body, JsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var res = _client.PostAsync(url, content).Result;
                var jsonResult = res.Content.ReadAsStringAsync().Result;

                if (!res.IsSuccessStatusCode || string.IsNullOrWhiteSpace(jsonResult))
                {
                    return new DataTableResponseDTO<SenderNumberListDTO>
                    {
                        draw = search.draw,
                        recordsTotal = 0,
                        recordsFiltered = 0,
                        data = new List<SenderNumberListDTO>(),
                        AdditionalData = jsonResult
                    };
                }

                var apiResponse =
                    JsonSerializer.Deserialize<ApiResponsePagedDTO<SenderNumberListDTO>>(jsonResult, JsonOptions);

                var total = apiResponse?.Meta?.TotalCount ?? 0;
                var data = apiResponse?.Data ?? new List<SenderNumberListDTO>();

                return new DataTableResponseDTO<SenderNumberListDTO>
                {
                    draw = search.draw,
                    recordsTotal = total,
                    recordsFiltered = total,
                    data = data
                };
            }
            catch (Exception ex)
            {
                return new DataTableResponseDTO<SenderNumberListDTO>
                {
                    draw = search.draw,
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = new List<SenderNumberListDTO>(),
                    AdditionalData = ex.Message
                };
            }
        }

        /* ----------------------------------------------
         *  سرشماره‌ها: GetById
         * ----------------------------------------------*/
        public SenderNumberEditDTO GetById(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                    return null;

                SetAuth();

                var url = $"{_baseUrl}/api/admin/sender-numbers/{id}";

                var res = _client.GetAsync(url).Result;
                if (!res.IsSuccessStatusCode)
                    return null;

                var jsonResult = res.Content.ReadAsStringAsync().Result;
                if (string.IsNullOrWhiteSpace(jsonResult))
                    return null;

                var apiResponse =
                    JsonSerializer.Deserialize<ApiResponseSingleDTO<SenderNumberEditDTO>>(
                        jsonResult, JsonOptions);

                return apiResponse?.Data;
            }
            catch
            {
                return null;
            }
        }

        /* ----------------------------------------------
         *  سرشماره‌ها: Create
         * ----------------------------------------------*/
        public BaseResult Create(SenderNumberCreateDTO model)
        {
            try
            {
                SetAuth();

                var url = $"{_baseUrl}/api/admin/sender-numbers";

                var body = new
                {
                    fixedPrefix = model.FixedPrefix,
                    specialtyId = model.SpecialtyId,
                    organizationLevelId = model.OrganizationLevelId,
                    subAreaId = model.SubAreaId,
                    status = model.Status,
                    description = model.Description
                };

                var json = JsonSerializer.Serialize(body, JsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var res = _client.PostAsync(url, content).Result;

                if (res.IsSuccessStatusCode)
                    return new BaseResult { Status = true, Message = "سرشماره با موفقیت ایجاد شد." };

                var msg = res.Content.ReadAsStringAsync().Result;
                return new BaseResult
                {
                    Status = false,
                    Message = string.IsNullOrWhiteSpace(msg) ? "خطا در ایجاد سرشماره." : msg
                };
            }
            catch (Exception ex)
            {
                return new BaseResult { Status = false, Message = ex.Message };
            }
        }

        /* ----------------------------------------------
         *  سرشماره‌ها: Update
         * ----------------------------------------------*/
        public BaseResult Update(SenderNumberEditDTO model)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(model.Id))
                    return new BaseResult { Status = false, Message = "شناسه سرشماره نامعتبر است." };

                SetAuth();

                var url = $"{_baseUrl}/api/admin/sender-numbers/{model.Id}";

                var body = new
                {
                    fixedPrefix = model.FixedPrefix,
                    specialtyId = model.SpecialtyId,
                    organizationLevelId = model.OrganizationLevelId,
                    subAreaId = model.SubAreaId,
                    status = model.Status,
                    description = model.Description
                };

                var json = JsonSerializer.Serialize(body, JsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var res = _client.PutAsync(url, content).Result;

                if (res.IsSuccessStatusCode)
                    return new BaseResult { Status = true, Message = "سرشماره با موفقیت ویرایش شد." };

                var msg = res.Content.ReadAsStringAsync().Result;
                return new BaseResult
                {
                    Status = false,
                    Message = string.IsNullOrWhiteSpace(msg) ? "خطا در ویرایش سرشماره." : msg
                };
            }
            catch (Exception ex)
            {
                return new BaseResult { Status = false, Message = ex.Message };
            }
        }

        /* ----------------------------------------------
         *  سرشماره‌ها: Delete
         * ----------------------------------------------*/
        public BaseResult Delete(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                    return new BaseResult { Status = false, Message = "شناسه سرشماره نامعتبر است." };

                SetAuth();

                var url = $"{_baseUrl}/api/admin/sender-numbers/{id}";

                var res = _client.DeleteAsync(url).Result;

                if (res.IsSuccessStatusCode)
                    return new BaseResult { Status = true, Message = "سرشماره با موفقیت حذف شد." };

                var msg = res.Content.ReadAsStringAsync().Result;
                return new BaseResult
                {
                    Status = false,
                    Message = string.IsNullOrWhiteSpace(msg) ? "خطا در حذف سرشماره." : msg
                };
            }
            catch (Exception ex)
            {
                return new BaseResult { Status = false, Message = ex.Message };
            }
        }

        /* ----------------------------------------------
         *  حوزه‌های تخصصی سرشماره
         *  /api/admin/sender-number-specialities/search
         * ----------------------------------------------*/
        public List<SenderNumberSpecialityListDTO> GetAllSpecialities()
        {
            try
            {
                SetAuth();

                var url = $"{_baseUrl}/api/admin/sender-number-specialties/search";

                var body = new
                {
                    page = 1,
                    pageSize = 200,
                    filters = new List<object>(),
                    sortBy = "code",
                    sortDescending = false
                };

                var jsonBody = JsonSerializer.Serialize(body, JsonOptions);
                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                var res = _client.PostAsync(url, content).Result;
                var jsonResult = res.Content.ReadAsStringAsync().Result;

                if (!res.IsSuccessStatusCode || string.IsNullOrWhiteSpace(jsonResult))
                    return new List<SenderNumberSpecialityListDTO>();

                var apiResponse =
                    JsonSerializer.Deserialize<ApiResponsePagedDTO<SenderNumberSpecialityListDTO>>(
                        jsonResult, JsonOptions);

                return apiResponse?.Data ?? new List<SenderNumberSpecialityListDTO>();
            }
            catch
            {
                return new List<SenderNumberSpecialityListDTO>();
            }
        }

        /* ----------------------------------------------
         *  رده‌های سازمانی سرشماره
         *  /api/admin/sender-number-organization-levels/search
         * ----------------------------------------------*/
        public List<SenderNumberOrganizationLevelListDTO> GetAllOrganizationLevels()
        {
            try
            {
                SetAuth();

                var url = $"{_baseUrl}/api/admin/sender-number-organization-levels/search";

                var body = new
                {
                    page = 1,
                    pageSize = 200,
                    filters = new List<object>(),
                    sortBy = "code",
                    sortDescending = false
                };

                var jsonBody = JsonSerializer.Serialize(body, JsonOptions);
                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                var res = _client.PostAsync(url, content).Result;
                var jsonResult = res.Content.ReadAsStringAsync().Result;

                if (!res.IsSuccessStatusCode || string.IsNullOrWhiteSpace(jsonResult))
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
         *  حوزه‌های فرعی سرشماره
         *  /api/admin/sender-number-sub-areas  (لیست ساده)
         * ----------------------------------------------*/
        public List<SenderNumberSubAreaListDTO> GetAllSubAreas()
        {
            try
            {
                SetAuth();

                var url = $"{_baseUrl}/api/admin/sender-number-sub-areas/search";

                var body = new
                {
                    page = 1,
                    pageSize = 200,
                    filters = new List<object>(),
                    sortBy = "code",
                    sortDescending = false
                };

                var jsonBody = JsonSerializer.Serialize(body, JsonOptions);
                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                var res = _client.PostAsync(url, content).Result;
                var jsonResult = res.Content.ReadAsStringAsync().Result;

                if (!res.IsSuccessStatusCode || string.IsNullOrWhiteSpace(jsonResult))
                    return new List<SenderNumberSubAreaListDTO>();

                var apiResponse =
                    JsonSerializer.Deserialize<ApiResponsePagedDTO<SenderNumberSubAreaListDTO>>(
                        jsonResult, JsonOptions);

                return apiResponse?.Data ?? new List<SenderNumberSubAreaListDTO>();


            }
            catch
            {
                return new List<SenderNumberSubAreaListDTO>();
            }
        }

        public BaseResult UpdateStatus(string id, string status)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(status))
                    return new BaseResult
                    {
                        Status = false,
                        Message = "شناسه یا وضعیت نامعتبر است."
                    };

                SetAuth();

                var url = $"{_baseUrl}/api/admin/sender-numbers/{id}/status";

                var body = new
                {
                    status = status  // مثلاً "Purchasing"
                };

                var json = JsonSerializer.Serialize(body, JsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var res = _client.PutAsync(url, content).Result;
                var msg = res.Content.ReadAsStringAsync().Result;

                if (res.IsSuccessStatusCode)
                    return new BaseResult
                    {
                        Status = true,
                        Message = "وضعیت سرشماره با موفقیت تغییر کرد."
                    };

                return new BaseResult
                {
                    Status = false,
                    Message = string.IsNullOrWhiteSpace(msg) ? "خطا در تغییر وضعیت سرشماره." : msg
                };
            }
            catch (Exception ex)
            {
                return new BaseResult
                {
                    Status = false,
                    Message = ex.Message
                };
            }
        }
    }
}
