using DTO.Base;
using DTO.DataTable;
using DTO.Project.SystemMenu;
using DTO.WebApi;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using static System.Net.WebRequestMethods;

namespace BLL.Project.SystemMenu
{
    public class SystemMenuManager : ISystemMenuManager
    {
        private readonly IHttpContextAccessor _httpContext;
        private readonly HttpClient _client;
        private readonly string _baseUrl;


        public SystemMenuManager(IHttpContextAccessor accessor, IConfiguration config)
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

        public List<SystemMenuListDTO> GetAll()
        {
            SetAuth();

            //var url = "http://87.107.111.44:8010/api/admin/Menu";
            var url = $"{_baseUrl}/Menu";
            var res = _client.GetAsync(url).Result;
            var json = res.Content.ReadAsStringAsync().Result;

            return JsonSerializer.Deserialize<List<SystemMenuListDTO>>(
                json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            ) ?? new List<SystemMenuListDTO>();
        }

        public DataTableResponseDTO<SystemMenuListDTO> GetDataTable(DataTableSearchDTO search)
        {
            SetAuth();

            //var url = "http://87.107.111.44:8010/api/admin/Menu/search";
            var url = $"{_baseUrl}/Menu/search";

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
            var responseJson = res.Content.ReadAsStringAsync().Result;

            if (!res.IsSuccessStatusCode)
            {
                return new DataTableResponseDTO<SystemMenuListDTO>
                {
                    draw = search.draw,
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = new List<SystemMenuListDTO>(),
                };
            }

            var api = JsonSerializer.Deserialize<ApiResponsePagedDTO<SystemMenuListDTO>>(
                responseJson,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );

            return new DataTableResponseDTO<SystemMenuListDTO>
            {
                draw = search.draw,
                recordsTotal = api.Meta.TotalCount,
                recordsFiltered = api.Meta.TotalCount,
                data = api.Data
            };
        }

        public BaseResult Create(SystemMenuCreateDTO model)
        {
            try
            {
                SetAuth();

                //var url = "http://87.107.111.44:8010/api/admin/Menu";
                 var url = $"{_baseUrl}/Menu";

                var json = JsonSerializer.Serialize(model);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var res = _client.PostAsync(url, content).Result;

                if (res.IsSuccessStatusCode)
                    return new BaseResult(true, "منو با موفقیت ایجاد شد.");

                return new BaseResult(false, res.Content.ReadAsStringAsync().Result);
            }
            catch (Exception ex)
            {
                return new BaseResult(false, ex.Message);
            }
        }

        public SystemMenuEditDTO GetById(string id)
        {
            try
            {
                SetAuth();

               // var url = $"http://87.107.111.44:8010/api/admin/Menu/{id}";
                var url = $"{_baseUrl}/Menu/{id}";

                var res = _client.GetAsync(url).Result;

                if (!res.IsSuccessStatusCode)
                    return null;

                var json = res.Content.ReadAsStringAsync().Result;

                var api = JsonSerializer.Deserialize<ApiResponseSingleDTO<SystemMenuEditDTO>>(
                    json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );

                return api.Data;
            }
            catch
            {
                return null;
            }
        }

        public BaseResult Update(SystemMenuEditDTO model)
        {
            try
            {
                SetAuth();

                //var url = $"http://87.107.111.44:8010/api/admin/Menu/{model.Id}";
                var url = $"{_baseUrl}/Menu/{model.Id}";

                var json = JsonSerializer.Serialize(model);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var res = _client.PutAsync(url, content).Result;

                if (res.IsSuccessStatusCode)
                    return new BaseResult(true, "ویرایش با موفقیت انجام شد.");
                return new BaseResult(false, res.Content.ReadAsStringAsync().Result);
            }
            catch (Exception ex)
            {
                return new BaseResult(false, ex.Message);
            }
        }

        public BaseResult Delete(string id)
        {
            try
            {
                SetAuth();

                //var url = $"http://87.107.111.44:8010/api/admin/Menu/{id}";
                var url = $"{_baseUrl}/Menu/{id}";
                var res = _client.DeleteAsync(url).Result;

                if (res.IsSuccessStatusCode)
                    return new BaseResult(true, "منو حذف شد.");

                return new BaseResult(false, res.Content.ReadAsStringAsync().Result);
            }
            catch (Exception ex)
            {
                return new BaseResult(false, ex.Message);
            }
        }


        public List<SelectListItem> GetParentMenusForDropdown()
        {
            SetAuth();

            var url = $"{_baseUrl}/Menu/search";

            var body = new
            {
                page = 1,
                pageSize = 500,
                filters = new List<object>(),
                sortBy = "Order",
                sortDescending = false
            };

            var json = JsonSerializer.Serialize(body);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var res = _client.PostAsync(url, content).Result;
            var responseJson = res.Content.ReadAsStringAsync().Result;

            if (!res.IsSuccessStatusCode)
                return new List<SelectListItem>();

            var api = JsonSerializer.Deserialize<ApiResponsePagedDTO<SystemMenuListDTO>>(
                responseJson,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );

            // ⭐ فقط منوهای بدون والد
            return api?.Data?
                .Where(x => string.IsNullOrWhiteSpace(x.ParentId))
                .OrderBy(x => x.Order)
                .Select(x => new SelectListItem
                {
                    Value = x.Id,
                    Text = x.Title
                })
                .ToList()
                ?? new List<SelectListItem>();
        }

    }
}

