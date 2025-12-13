using BLL.Project.SenderNumberSubArea;
using DocumentFormat.OpenXml.Spreadsheet;
using DTO.Base;
using DTO.DataTable;
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

namespace BLL.Project.SenderNumberSpeciality
{
    public class SenderNumberSpecialityManager : ISenderNumberSpecialityManager
    {
        private readonly IHttpContextAccessor _httpContext;
        private readonly HttpClient _client;
        private readonly string _baseUrl;


        public SenderNumberSpecialityManager(IHttpContextAccessor accessor, IConfiguration config)
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



        public DataTableResponseDTO<SenderNumberSpecialityListDTO> GetDataTableDTO(DataTableSearchDTO search)
        {
            SetAuth();

           // var url = "http://87.107.111.44:8010/api/admin/sender-number-specialties/search";
            var url = $"{_baseUrl}/sender-number-specialties/search";
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
                return new DataTableResponseDTO<SenderNumberSpecialityListDTO>
                {
                    draw = search.draw,
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = new List<SenderNumberSpecialityListDTO>(),
                    AdditionalData = jsonResult
                };
            }

            var apiResponse = JsonSerializer.Deserialize<ApiResponsePagedDTO<SenderNumberSpecialityListDTO>>(jsonResult,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return new DataTableResponseDTO<SenderNumberSpecialityListDTO>
            {
                draw = search.draw,
                recordsTotal = apiResponse.Meta.TotalCount,
                recordsFiltered = apiResponse.Meta.TotalCount,
                data = apiResponse.Data
            };
        }

        public BaseResult Create(SenderNumberSpecialityCreateDTO model)
        {
            try
            {
                SetAuth(); // حالا توکن ست می‌شود

               // var url = "http://87.107.111.44:8010/api/admin/sender-number-specialties";
                var url = $"{_baseUrl}/sender-number-specialties";

                var json = JsonSerializer.Serialize(model);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var res = _client.PostAsync(url, content).Result;

                if (res.IsSuccessStatusCode)
                    return new BaseResult { Status = true, Message = "با موفقیت ثبت شد." };

                return new BaseResult
                {
                    Status = false,
                    Message = res.Content.ReadAsStringAsync().Result
                };
            }
            catch (Exception ex)
            {
                return new BaseResult { Status = false, Message = ex.Message };
            }
        }


        public SenderNumberSubAreaEditDTO GetByCode(string code)
        {
            var all = GetAll();
            return all
                .Where(x => x.Code == code)
                .Select(x => new SenderNumberSubAreaEditDTO
                {
                    Code = x.Code,
                    Title = x.Title,
                    Description = x.Description
                })
                .FirstOrDefault();
        }

        public BaseResult Update(SenderNumberSpecialityEditDTO model)
        {
            try
            {
                var token = _httpContext.HttpContext.Session.GetString("AdminToken");

                using var http = new HttpClient(); // مهم!
                http.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);

               // var url = $"http://87.107.111.44:8010/api/admin/sender-number-specialties/{model.Id}";
                var url = $"{_baseUrl}/sender-number-specialties/{model.Id}";

                var body = new
                {
                    code = model.code,
                    title = model.title,
                    description = model.description
                };

                var json = JsonSerializer.Serialize(body);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var res = http.PutAsync(url, content).Result;

                if (res.IsSuccessStatusCode)
                    return new BaseResult { Status = true, Message = "با موفقیت ویرایش شد." };

                return new BaseResult { Status = false, Message = res.Content.ReadAsStringAsync().Result };
            }
            catch (Exception ex)
            {
                return new BaseResult { Status = false, Message = ex.Message };
            }
        }


        public SenderNumberSpecialityEditDTO GetById(string id)
        {
            try
            {
                SetAuth();

               // var url = $"http://87.107.111.44:8010/api/admin/sender-number-specialties/{id}";
                var url = $"{_baseUrl}/sender-number-specialties/{id}";

                var res = _client.GetAsync(url).Result;

                if (!res.IsSuccessStatusCode)
                    return null;

                var json = res.Content.ReadAsStringAsync().Result;

                var apiResponse = JsonSerializer.Deserialize<ApiResponseSingleDTO<SenderNumberSpecialityEditDTO>>(
                    json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );

                return apiResponse?.Data;
            }
            catch
            {
                return null;
            }
        }

        public List<SenderNumberSpecialityListDTO> GetAll()
        {
            SetAuth();

           // var url = "http://87.107.111.44:8010/api/admin/sender-number-sub-areas";
            var url = $"{_baseUrl}/sender-number-sub-areas";
            var res = _client.GetAsync(url).Result;

            var json = res.Content.ReadAsStringAsync().Result;

            return JsonSerializer.Deserialize<List<SenderNumberSpecialityListDTO>>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                ?? new List<SenderNumberSpecialityListDTO>();
        }

        /* ----------------------------------------------
         *  حذف زیربخش
         * ----------------------------------------------*/
        public BaseResult Delete(string code)
        {
            try
            {
                SetAuth();

                //var url = $"http://87.107.111.44:8010/api/admin/sender-number-sub-areas/{code}";
                var url = $"{_baseUrl}/sender-number-sub-areas/{code}";

                var res = _client.DeleteAsync(url).Result;

                if (res.IsSuccessStatusCode)
                    return new BaseResult { Status = true, Message = "با موفقیت حذف شد." };

                return new BaseResult
                {
                    Status = false,
                    Message = res.Content.ReadAsStringAsync().Result
                };
            }
            catch (Exception ex)
            {
                return new BaseResult { Status = false, Message = ex.Message };
            }
        }
    }
}
