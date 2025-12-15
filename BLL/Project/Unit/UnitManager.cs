using DTO.Base;
using DTO.DataTable;
using DTO.Project.Unit;
using DTO.Project.WebApi;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BLL.Project.Unit
{
    public class UnitManager : IUnitManager
    {
        private readonly IHttpContextAccessor _httpContext;
        private readonly HttpClient _client;
        private readonly string _baseUrl;

        public UnitManager(IHttpContextAccessor accessor, IConfiguration config)
        {
            _httpContext = accessor;
            _client = new HttpClient();

            _baseUrl ="http://87.107.111.44:8010";
            //_baseUrl = config["ApiBaseUrl"] ?? "http://87.107.111.44:8010";
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

        /// <summary>
        /// گرفتن همه واحدها (با یک سرچ با pageSize بزرگ)
        /// </summary>
        public List<UnitListDTO> GetAll()
        {
            try
            {
                SetAuth();

                var url = $"{_baseUrl}/api/admin/units/search";

                var request = new PagedFilterRequestDTO
                {
                    Page = 1,
                    PageSize = 1000,        // اگر روزی زیاد شد، باید سرور پشتیبانی کند
                    Filters = new List<FilterRuleDTO>(),
                    SortBy = "code",
                    SortDescending = false
                };

                var json = JsonSerializer.Serialize(request, JsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var res = _client.PostAsync(url, content).Result;
                var jsonResult = res.Content.ReadAsStringAsync().Result;

                if (!res.IsSuccessStatusCode)
                    return new List<UnitListDTO>();

                var apiResponse = JsonSerializer.Deserialize<UnitListApiResponseDTO>(jsonResult, JsonOptions);

                return apiResponse?.Data ?? new List<UnitListDTO>();
            }
            catch
            {
                return new List<UnitListDTO>();
            }
        }

        /// <summary>
        /// خروجی مناسب DataTable (سرور-ساید شبیه‌سازی شده روی نتیجه search)
        /// </summary>
        public DataTableResponseDTO<UnitListDTO> GetDataTableDTO(DataTableSearchDTO search)
        {
            SetAuth();

            var url = $"{_baseUrl}/api/admin/units/search";

            // فعلاً بدون فیلتر (اگر خواستی سرچ روی name/code اضافه می‌کنیم)
            var request = new PagedFilterRequestDTO
            {
                Page = 1,
                PageSize = 200,
                Filters = new List<FilterRuleDTO>(),
                SortBy = search.sortColumnName,
                SortDescending = search.sortDirection == "desc"
            };

            var json = JsonSerializer.Serialize(request, JsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var res = _client.PostAsync(url, content).Result;
            var jsonResult = res.Content.ReadAsStringAsync().Result;

            if (!res.IsSuccessStatusCode)
            {
                return new DataTableResponseDTO<UnitListDTO>
                {
                    draw = search.draw,
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = new List<UnitListDTO>(),
                    AdditionalData = jsonResult
                };
            }

            var apiResponse = JsonSerializer.Deserialize<UnitListApiResponseDTO>(jsonResult, JsonOptions);
            var all = apiResponse?.Data ?? new List<UnitListDTO>();

            // فیلتر ساده روی searchValue (روی کد یا نام)
            if (!string.IsNullOrWhiteSpace(search.searchValue))
            {
                var term = search.searchValue.Trim();
                all = all
                    .Where(x =>
                        (x.Code != null && x.Code.Contains(term, StringComparison.OrdinalIgnoreCase)) ||
                        (x.Name != null && x.Name.Contains(term, StringComparison.OrdinalIgnoreCase)))
                    .ToList();
            }

            var total = all.Count;

            // صفحه‌بندی دستی
            var paged = all
                .Skip(search.start)
                .Take(search.length)
                .ToList();

            return new DataTableResponseDTO<UnitListDTO>
            {
                draw = search.draw,
                recordsTotal = total,
                recordsFiltered = total,
                data = paged
            };
        }

        public BaseResult Create(UnitCreateDTO model)
        {
            try
            {
                SetAuth();

                var url = $"{_baseUrl}/api/admin/units";

                // مطابق UnitRequestDto در OpenAPI
                
                var body = new
                {
                    code = model.Code,
                    name = model.Name,
                    description = model.Description,
                    isActive = model.IsActive,
                    parentId = (string)null 
                };

                var json = JsonSerializer.Serialize(body, JsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var res = _client.PostAsync(url, content).Result;

                if (res.IsSuccessStatusCode)
                    return new BaseResult { Status = true, Message = "واحد با موفقیت ثبت شد." };

                var msg = res.Content.ReadAsStringAsync().Result;
                return new BaseResult { Status = false, Message = msg };
            }
            catch (Exception ex)
            {
                return new BaseResult { Status = false, Message = ex.Message };
            }
        }

        public UnitEditDTO? GetById(string id)
        {
            try
            {
                SetAuth();

                var url = $"{_baseUrl}/api/admin/units/search";

                var request = new PagedFilterRequestDTO
                {
                    Page = 1,
                    PageSize = 1,
                    Filters = new List<FilterRuleDTO>
                    {
                        new FilterRuleDTO
                        {
                            Field = "id",
                            Operator = FilterOperator.Equals,
                            Value = id
                        }
                    },
                    SortBy = "code",
                    SortDescending = false
                };

                var json = JsonSerializer.Serialize(request, JsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var res = _client.PostAsync(url, content).Result;
                var jsonResult = res.Content.ReadAsStringAsync().Result;

                if (!res.IsSuccessStatusCode)
                    return null;

                var apiResponse = JsonSerializer.Deserialize<UnitListApiResponseDTO>(jsonResult, JsonOptions);
                var unit = apiResponse?.Data?.FirstOrDefault();
                if (unit == null)
                    return null;

                return new UnitEditDTO
                {
                    Id = unit.Id,
                    Code = unit.Code,
                    Name = unit.Name,
                    Description = unit.Description,
                    IsActive = unit.IsActive,
                    //ParentId = unit.ParentId
                };
            }
            catch
            {
                return null;
            }
        }

        public BaseResult Update(UnitEditDTO model)
        {
            try
            {
                SetAuth();

                var url = $"{_baseUrl}/api/admin/units/{model.Id}";

                var body = new
                {
                    code = model.Code,
                    name = model.Name,
                    description = model.Description,
                    isActive = model.IsActive,
                    //parentId = string.IsNullOrWhiteSpace(model.ParentId) ? null : model.ParentId
                };

                var json = JsonSerializer.Serialize(body, JsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var res = _client.PutAsync(url, content).Result;

                if (res.IsSuccessStatusCode)
                    return new BaseResult { Status = true, Message = "واحد با موفقیت ویرایش شد." };

                var msg = res.Content.ReadAsStringAsync().Result;
                return new BaseResult { Status = false, Message = msg };
            }
            catch (Exception ex)
            {
                return new BaseResult { Status = false, Message = ex.Message };
            }
        }

        public List<SelectListItem> GetUnitsForDropdown()
        {
            SetAuth();

            var url = $"{_baseUrl}/api/admin/units/search";

            var request = new PagedFilterRequestDTO
            {
                Page = 1,
                PageSize = 200,
                Filters = new List<FilterRuleDTO>(),
                SortBy = "Name",
                SortDescending = false
            };

            var json = JsonSerializer.Serialize(request, JsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var res = _client.PostAsync(url, content).Result;
            if (!res.IsSuccessStatusCode)
                return new List<SelectListItem>();

            var jsonResult = res.Content.ReadAsStringAsync().Result;
            var apiResponse =
                JsonSerializer.Deserialize<UnitListApiResponseDTO>(jsonResult, JsonOptions);

            return apiResponse?.Data?
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),   // Guid → string
                    Text = $"{x.Name} ({x.Code})"
                })
                .ToList()
                ?? new List<SelectListItem>();
        }

    }
}
