using DTO.Base;
using DTO.DataTable;
using DTO.Project.SenderNumberSubAreaList;
using DTO.Project.User;
using DTO.WebApi;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using static System.Net.WebRequestMethods;

namespace BLL.Project.User
{
    public class UserManager : IUserManager
    {
        private readonly IHttpContextAccessor _httpContext;
        private readonly HttpClient _client;
        //  private readonly string _baseUrl;


        public UserManager(IHttpContextAccessor accessor, IConfiguration config)
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

        public List<SystemUserListDTO> GetAll()
        {
            SetAuth();

            var url = "http://87.107.111.44:8010/api/admin/users";
            var res = _client.GetAsync(url).Result;

            var json = res.Content.ReadAsStringAsync().Result;

            return JsonSerializer.Deserialize<List<SystemUserListDTO>>(
                json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            ) ?? new List<SystemUserListDTO>();
        }

        public DataTableResponseDTO<SystemUserListDTO> GetDataTable(DataTableSearchDTO search)
        {
            SetAuth();

            var url = "http://87.107.111.44:8010/api/admin/users/search";

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
                return new DataTableResponseDTO<SystemUserListDTO>
                {
                    draw = search.draw,
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = new List<SystemUserListDTO>(),

                };
            }

            var apiResponse = JsonSerializer.Deserialize<ApiResponsePagedDTO<SystemUserListDTO>>(
                responseJson,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );

            return new DataTableResponseDTO<SystemUserListDTO>
            {
                draw = search.draw,
                recordsTotal = apiResponse.Meta.TotalCount,
                recordsFiltered = apiResponse.Meta.TotalCount,
                data = apiResponse.Data
            };
        }

        public BaseResult Create(SystemUserCreateDTO model)
        {
            try
            {
                SetAuth();

                var url = "http://87.107.111.44:8010/api/admin/users";

                var json = JsonSerializer.Serialize(model);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var res = _client.PostAsync(url, content).Result;

                if (res.IsSuccessStatusCode)
                    return new BaseResult(true, "کاربر با موفقیت ثبت شد.");

                return new BaseResult(false, res.Content.ReadAsStringAsync().Result);
            }
            catch (Exception ex)
            {
                return new BaseResult(false, ex.Message);
            }
        }

        public SystemUserEditDTO GetById(string id)
        {
            try
            {
                SetAuth();

                var url = $"http://87.107.111.44:8010/api/admin/users/{id}";

                var res = _client.GetAsync(url).Result;

                if (!res.IsSuccessStatusCode)
                    return null;

                var json = res.Content.ReadAsStringAsync().Result;

                var apiResponse =
                    JsonSerializer.Deserialize<ApiResponseSingleDTO<SystemUserEditDTO>>(
                        json,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                    );

                return apiResponse.Data;
            }
            catch
            {
                return null;
            }
        }

        public BaseResult Update(SystemUserEditDTO model)
        {
            try
            {
                SetAuth();

                var url = $"http://87.107.111.44:8010/api/admin/users/{model.Id}";

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

                var url = $"http://87.107.111.44:8010/api/admin/users/{id}";
                var res = _client.DeleteAsync(url).Result;

                if (res.IsSuccessStatusCode)
                    return new BaseResult(true, "کاربر حذف شد.");

                return new BaseResult(false, res.Content.ReadAsStringAsync().Result);
            }
            catch (Exception ex)
            {
                return new BaseResult(false, ex.Message);
            }
        }
    }
}
