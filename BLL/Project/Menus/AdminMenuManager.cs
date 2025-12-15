using DTO.Base;
using DTO.DataTable;
using DTO.Project.Menus;
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

namespace BLL.Project.Menus
{
    public class AdminMenuManager : IAdminMenuManager
    {
        private readonly IHttpContextAccessor _httpContext;
        private readonly HttpClient _client;
        private readonly string _baseUrl;

        public AdminMenuManager(IHttpContextAccessor accessor, IConfiguration config)
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
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        private JsonSerializerOptions JsonOptions => new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public List<MenuTreeNodeDTO> GetTree()
        {
            try
            {
                SetAuth();
                var url = $"{_baseUrl}/api/admin/Menu/search";
                var body = new
                {
                    page = 1,
                    pageSize = 200,
                    filters = new List<object>(),
                    sortBy = "order",
                    sortDescending = false
                };

                var json = JsonSerializer.Serialize(body, JsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var res = _client.PostAsync(url, content).Result;
                var jsonResult = res.Content.ReadAsStringAsync().Result;

                if (!res.IsSuccessStatusCode || string.IsNullOrWhiteSpace(jsonResult))
                    return new List<MenuTreeNodeDTO>();

                var apiResponse = JsonSerializer.Deserialize<ApiResponsePagedDTO<MenuTreeNodeDTO>>(jsonResult, JsonOptions);

                // تبدیل لیست به درخت
                var allItems = apiResponse?.Data ?? new List<MenuTreeNodeDTO>();
                return BuildTree(allItems);
            }
            catch
            {
                return new List<MenuTreeNodeDTO>();
            }
        }

        private List<MenuTreeNodeDTO> BuildTree(List<MenuTreeNodeDTO> items)
        {
            var tree = new List<MenuTreeNodeDTO>();
            var lookup = items.ToDictionary(x => x.Id, x => x);

            foreach (var item in items)
            {
                if (string.IsNullOrEmpty(item.ParentId))
                {
                    tree.Add(item);
                }
                else if (lookup.ContainsKey(item.ParentId))
                {
                    lookup[item.ParentId].Children.Add(item);
                }
            }

            // مرتب‌سازی فرزندان بر اساس Order
            foreach (var node in lookup.Values)
            {
                node.Children = node.Children.OrderBy(c => c.Order).ToList();
            }

            return tree.OrderBy(x => x.Order).ToList();
        }

        public DataTableResponseDTO<MenuListDTO> GetDataTableDTO(DataTableSearchDTO search)
        {
            try
            {
                SetAuth();
                var url = $"{_baseUrl}/api/admin/Menu/search";
                var body = new
                {
                    page = (search.start / search.length) + 1,
                    pageSize = search.length,
                    filters = new List<object>(),
                    sortBy = string.IsNullOrWhiteSpace(search.sortColumnName) ? "order" : search.sortColumnName,
                    sortDescending = search.sortDirection == "desc"
                };

                var json = JsonSerializer.Serialize(body, JsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var res = _client.PostAsync(url, content).Result;
                var jsonResult = res.Content.ReadAsStringAsync().Result;

                if (!res.IsSuccessStatusCode || string.IsNullOrWhiteSpace(jsonResult))
                {
                    return new DataTableResponseDTO<MenuListDTO>
                    {
                        draw = search.draw,
                        recordsTotal = 0,
                        recordsFiltered = 0,
                        data = new List<MenuListDTO>(),
                        AdditionalData = jsonResult
                    };
                }

                var apiResponse = JsonSerializer.Deserialize<ApiResponsePagedDTO<MenuTreeNodeDTO>>(jsonResult, JsonOptions);
                var total = apiResponse?.Meta?.TotalCount ?? 0;

                // تبدیل درخت به لیست مسطح برای جدول
                var treeData = apiResponse?.Data ?? new List<MenuTreeNodeDTO>();
                var flatList = FlattenTree(treeData);

                // جستجو در داده‌ها
                if (!string.IsNullOrWhiteSpace(search.searchValue))
                {
                    var searchLower = search.searchValue.ToLower();
                    flatList = flatList.Where(x =>
                        (x.Title?.ToLower().Contains(searchLower) ?? false) ||
                        (x.Key?.ToLower().Contains(searchLower) ?? false) ||
                        (x.RouteKey?.ToLower().Contains(searchLower) ?? false)
                    ).ToList();
                }

                // مرتب‌سازی
                if (!string.IsNullOrWhiteSpace(search.sortColumnName))
                {
                    var property = typeof(MenuListDTO).GetProperty(search.sortColumnName);
                    if (property != null)
                    {
                        flatList = search.sortDirection == "asc"
                            ? flatList.OrderBy(x => property.GetValue(x, null)).ToList()
                            : flatList.OrderByDescending(x => property.GetValue(x, null)).ToList();
                    }
                }

                return new DataTableResponseDTO<MenuListDTO>
                {
                    draw = search.draw,
                    recordsTotal = total,
                    recordsFiltered = flatList.Count,
                    data = flatList.Skip(search.start).Take(search.length).ToList()
                };
            }
            catch (Exception ex)
            {
                return new DataTableResponseDTO<MenuListDTO>
                {
                    draw = search.draw,
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = new List<MenuListDTO>(),
                    AdditionalData = ex.Message
                };
            }
        }

        private List<MenuListDTO> FlattenTree(List<MenuTreeNodeDTO> nodes, int level = 0, string parentTitle = null)
        {
            var result = new List<MenuListDTO>();
            foreach (var node in nodes)
            {
                var item = new MenuListDTO
                {
                    Id = node.Id,
                    Key = node.Key,
                    Title = new string('—', level * 2) + " " + node.Title,
                    Icon = node.Icon,
                    RouteKey = node.RouteKey,
                    Order = node.Order,
                    ParentId = node.ParentId,
                    ParentTitle = parentTitle,
                    IsActive = node.IsActive
                };
                result.Add(item);

                // اضافه کردن فرزندان
                if (node.Children != null && node.Children.Any())
                {
                    result.AddRange(FlattenTree(node.Children, level + 1, node.Title));
                }
            }
            return result;
        }

        public List<MenuListDTO> GetAll()
        {
            try
            {
                SetAuth();
                var url = $"{_baseUrl}/api/admin/Menu/search";
                var body = new
                {
                    page = 1,
                    pageSize = 200,
                    filters = new List<object>(),
                    sortBy = "title",
                    sortDescending = false
                };

                var json = JsonSerializer.Serialize(body, JsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var res = _client.PostAsync(url, content).Result;
                var jsonResult = res.Content.ReadAsStringAsync().Result;

                if (!res.IsSuccessStatusCode || string.IsNullOrWhiteSpace(jsonResult))
                    return new List<MenuListDTO>();

                var apiResponse = JsonSerializer.Deserialize<ApiResponsePagedDTO<MenuTreeNodeDTO>>(jsonResult, JsonOptions);
                var treeData = apiResponse?.Data ?? new List<MenuTreeNodeDTO>();
                return FlattenTree(treeData, 0);
            }
            catch
            {
                return new List<MenuListDTO>();
            }
        }

        public MenuEditDTO GetById(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                    return null;

                SetAuth();
                var url = $"{_baseUrl}/api/admin/Menu/{id}";
                var res = _client.GetAsync(url).Result;

                if (!res.IsSuccessStatusCode)
                    return null;

                var jsonResult = res.Content.ReadAsStringAsync().Result;
                if (string.IsNullOrWhiteSpace(jsonResult))
                    return null;

                var apiResponse = JsonSerializer.Deserialize<ApiResponseSingleDTO<MenuTreeNodeDTO>>(jsonResult, JsonOptions);
                var treeNode = apiResponse?.Data;

                if (treeNode == null)
                    return null;

                return new MenuEditDTO
                {
                    Id = id,
                    Key = treeNode.Key,
                    Title = treeNode.Title,
                    Icon = treeNode.Icon,
                    RouteKey = treeNode.RouteKey,
                    Order = treeNode.Order,
                    ParentId = treeNode.ParentId,
                    IsActive = treeNode.IsActive
                };
            }
            catch
            {
                return null;
            }
        }

        public BaseResult Create(MenuCreateDTO model)
        {
            try
            {
                SetAuth();
                var url = $"{_baseUrl}/api/admin/Menu";
                var body = new
                {
                    key = model.Key,
                    title = model.Title,
                    icon = model.Icon,
                    routeKey = model.RouteKey,
                    order = model.Order,
                    parentId = string.IsNullOrWhiteSpace(model.ParentId) ? null : model.ParentId,
                    isActive = model.IsActive
                };

                var json = JsonSerializer.Serialize(body, JsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var res = _client.PostAsync(url, content).Result;

                if (res.IsSuccessStatusCode)
                    return new BaseResult { Status = true, Message = "منو با موفقیت ایجاد شد." };

                var msg = res.Content.ReadAsStringAsync().Result;
                return new BaseResult { Status = false, Message = string.IsNullOrWhiteSpace(msg) ? "خطا در ایجاد منو." : msg };
            }
            catch (Exception ex)
            {
                return new BaseResult { Status = false, Message = ex.Message };
            }
        }

        public BaseResult Update(MenuEditDTO model)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(model.Id))
                    return new BaseResult { Status = false, Message = "شناسه منو نامعتبر است." };

                SetAuth();
                var url = $"{_baseUrl}/api/admin/Menu/{model.Id}";
                var body = new
                {
                    id = model.Id,
                    key = model.Key,
                    title = model.Title,
                    icon = model.Icon,
                    routeKey = model.RouteKey,
                    order = model.Order,
                    parentId = string.IsNullOrWhiteSpace(model.ParentId) ? null : model.ParentId,
                    isActive = model.IsActive
                };

                var json = JsonSerializer.Serialize(body, JsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var res = _client.PutAsync(url, content).Result;

                if (res.IsSuccessStatusCode)
                    return new BaseResult { Status = true, Message = "منو با موفقیت ویرایش شد." };

                var msg = res.Content.ReadAsStringAsync().Result;
                return new BaseResult { Status = false, Message = string.IsNullOrWhiteSpace(msg) ? "خطا در ویرایش منو." : msg };
            }
            catch (Exception ex)
            {
                return new BaseResult { Status = false, Message = ex.Message };
            }
        }

        public BaseResult Delete(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                    return new BaseResult { Status = false, Message = "شناسه منو نامعتبر است." };

                SetAuth();
                var url = $"{_baseUrl}/api/admin/Menu/{id}";
                var res = _client.DeleteAsync(url).Result;

                if (res.IsSuccessStatusCode)
                    return new BaseResult { Status = true, Message = "منو با موفقیت حذف شد." };

                var msg = res.Content.ReadAsStringAsync().Result;
                return new BaseResult { Status = false, Message = string.IsNullOrWhiteSpace(msg) ? "خطا در حذف منو." : msg };
            }
            catch (Exception ex)
            {
                return new BaseResult { Status = false, Message = ex.Message };
            }
        }
    }
}
