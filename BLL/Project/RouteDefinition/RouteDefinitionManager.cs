using DTO.Base;
using DTO.Project.RouteDefinition;
using DTO.WebApi;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace BLL.Project.RouteDefinition
{
    /// <summary>
    /// تعریف مجوز
    /// </summary>
    public class RouteDefinitionManager : IRouteDefinitionManager
    {
        private readonly IHttpContextAccessor _httpContext;
        private readonly HttpClient _client;
        private readonly string _baseUrl;


        public RouteDefinitionManager(IHttpContextAccessor accessor, IConfiguration config)
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
            {
                _client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }
        }

        public List<RouteDefinitionListDTO> GetAll()
        {
            try
            {
                SetAuth();

                var url = $"{_baseUrl}/route-definitions";
                var res = _client.GetAsync(url).Result;

                if (!res.IsSuccessStatusCode)
                    return new List<RouteDefinitionListDTO>();

                var json = res.Content.ReadAsStringAsync().Result;

                var result = JsonSerializer.Deserialize<ApiResponsePagedDTO<RouteDefinitionListDTO>>(
                    json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );

                return result?.Data ?? new List<RouteDefinitionListDTO>();
            }
            catch
            {
                return new List<RouteDefinitionListDTO>();
            }
        }



        public BaseResult Update(string routeKey, RouteDefinitionUpdateDTO model)
        {
            try
            {
                SetAuth();

                var url = $"{_baseUrl}/route-definitions/{routeKey}";

                var json = JsonSerializer.Serialize(model);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var res = _client.PutAsync(url, content).Result;

                if (res.IsSuccessStatusCode)
                    return new BaseResult(true, "ویرایش Route با موفقیت انجام شد.");

                return new BaseResult(false, res.Content.ReadAsStringAsync().Result);
            }
            catch (Exception ex)
            {
                return new BaseResult(false, ex.Message);
            }
        }

        public BaseResult Delete(string routeKey)
        {
            try
            {
                SetAuth();

                var url = $"{_baseUrl}/route-definitions/{routeKey}";
                var res = _client.DeleteAsync(url).Result;

                if (res.IsSuccessStatusCode)
                    return new BaseResult(true, "Route با موفقیت حذف شد.");

                return new BaseResult(false, res.Content.ReadAsStringAsync().Result);
            }
            catch (Exception ex)
            {
                return new BaseResult(false, ex.Message);
            }
        }
    }
}
