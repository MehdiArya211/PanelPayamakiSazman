using DTO.DataTable;
using DTO.Project.LoginAttempt;
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

namespace BLL.Project.LoginAttempt
{
    public class LoginAttemptManager : ILoginAttemptManager
    {
        private readonly IHttpContextAccessor _httpContext;
        private readonly HttpClient _client;
        private readonly string _baseUrl;


        public LoginAttemptManager(IHttpContextAccessor accessor, IConfiguration config)
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

        public DataTableResponseDTO<LoginAttemptListDTO> GetLoginAttemptsDataTable(DataTableSearchDTO search)

        {
            SetAuth();

            var url = $"{_baseUrl}/audit/login-attempts/search";

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
                return new DataTableResponseDTO<LoginAttemptListDTO>
                {
                    draw = search.draw,
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = new List<LoginAttemptListDTO>()
                };
            }

            var apiResponse =
                JsonSerializer.Deserialize<ApiResponsePagedDTO<LoginAttemptListDTO>>(
                    responseJson,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return new DataTableResponseDTO<LoginAttemptListDTO>
            {
                draw = search.draw,
                recordsTotal = apiResponse.Meta.TotalCount,
                recordsFiltered = apiResponse.Meta.TotalCount,
                data = apiResponse.Data
            };
        }


        public DataTableResponseDTO<SecurityQuestionAttemptListDTO> GetSecurityQuestionAttemptsDataTable(DataTableSearchDTO search)

        {
            SetAuth();

            var url = $"{_baseUrl}/audit/security-question-attempts/search";

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
                return new DataTableResponseDTO<SecurityQuestionAttemptListDTO>
                {
                    draw = search.draw,
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = new List<SecurityQuestionAttemptListDTO>()
                };
            }

            var apiResponse =
                JsonSerializer.Deserialize<ApiResponsePagedDTO<SecurityQuestionAttemptListDTO>>(
                    responseJson,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return new DataTableResponseDTO<SecurityQuestionAttemptListDTO>
            {
                draw = search.draw,
                recordsTotal = apiResponse.Meta.TotalCount,
                recordsFiltered = apiResponse.Meta.TotalCount,
                data = apiResponse.Data
            };
        }


    }
}
