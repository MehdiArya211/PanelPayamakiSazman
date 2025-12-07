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
        //  private readonly string _baseUrl;


        public SenderNumberSpecialityManager(IHttpContextAccessor accessor, IConfiguration config)
        {
            _httpContext = accessor;
            _client = new HttpClient();
            //  _baseUrl = config["ApiBaseUrl"];

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

            var url = "http://87.107.111.44:8010/api/admin/sender-number-specialties/search";
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

                var url = "http://87.107.111.44:8010/api/admin/sender-number-specialties";

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
    }
}
