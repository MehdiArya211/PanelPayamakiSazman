using DTO.Base;
using DTO.DataTable;
using DTO.Project.SenderNumberSubAreaList;
using DTO.WebApi;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Services.SessionServices;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace BLL.Project.SenderNumberSubArea
{
    public class SenderNumberSubAreaManager : ISenderNumberSubAreaManager
    {
        private readonly IHttpContextAccessor _httpContext;
        private readonly HttpClient _client;
      //  private readonly string _baseUrl;


        public SenderNumberSubAreaManager(IHttpContextAccessor accessor, IConfiguration config)
        {
            _httpContext = accessor;
            _client = new HttpClient();
          //  _baseUrl = config["ApiBaseUrl"];

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

        public BaseResult Create(SenderNumberSubAreaCreateDTO model)
        {
            try
            {
                SetAuth(); // حالا توکن ست می‌شود

                var url = "http://87.107.111.44:8010/api/admin/sender-number-sub-areas";

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

        public List<SenderNumberSubAreaListDTO> GetAll()
        {
            SetAuth();

            var url = "http://87.107.111.44:8010/api/admin/sender-number-sub-areas";
            var res = _client.GetAsync(url).Result;

            var json = res.Content.ReadAsStringAsync().Result;

            return JsonSerializer.Deserialize<List<SenderNumberSubAreaListDTO>>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                ?? new List<SenderNumberSubAreaListDTO>();
        }

        /* ----------------------------------------------
         *  گرفتن یک آیتم برای ویرایش
         * ----------------------------------------------*/
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

        public BaseResult Update(SenderNumberSubAreaEditDTO model)
        {
            try
            {
                var token = _httpContext.HttpContext.Session.GetString("AdminToken");

                using var http = new HttpClient(); // مهم!
                http.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);

                var url = $"http://87.107.111.44:8010/api/admin/sender-number-sub-areas/{model.Id}";

                var body = new
                {
                    code = model.Code,
                    title = model.Title,
                    description = model.Description
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


        public SenderNumberSubAreaEditDTO GetById(string id)
        {
            try
            {
                SetAuth();

                var url = $"http://87.107.111.44:8010/api/admin/sender-number-sub-areas/{id}";

                var res = _client.GetAsync(url).Result;

                if (!res.IsSuccessStatusCode)
                    return null;

                var json = res.Content.ReadAsStringAsync().Result;

                var apiResponse = JsonSerializer.Deserialize<ApiResponseSingleDTO<SenderNumberSubAreaEditDTO>>(
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



        /* ----------------------------------------------
         *  حذف زیربخش
         * ----------------------------------------------*/
        public BaseResult Delete(string code)
        {
            try
            {
                SetAuth();

                var url = $"http://87.107.111.44:8010/api/admin/sender-number-sub-areas/{code}";

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


        /* ----------------------------------------------
         *  خروجی دیتاتیبل
         * ----------------------------------------------*/

        public DataTableResponseDTO<SenderNumberSubAreaListDTO> GetDataTableDTO(DataTableSearchDTO search)
        {
            SetAuth();

            var url = "http://87.107.111.44:8010/api/admin/sender-number-sub-areas/search";

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
                return new DataTableResponseDTO<SenderNumberSubAreaListDTO>
                {
                    draw = search.draw,
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = new List<SenderNumberSubAreaListDTO>(),
                    AdditionalData = jsonResult
                };
            }

            var apiResponse = JsonSerializer.Deserialize<ApiResponsePagedDTO<SenderNumberSubAreaListDTO>>(jsonResult,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return new DataTableResponseDTO<SenderNumberSubAreaListDTO>
            {
                draw = search.draw,
                recordsTotal = apiResponse.Meta.TotalCount,
                recordsFiltered = apiResponse.Meta.TotalCount,
                data = apiResponse.Data
            };
        }

    }
}




