using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Domain.Entities;
using DTO;
using DTO.User;
using Infrastructure.Data;

namespace BLL
{
    public class UnitsManager : Manager<User, ApplicationContext>, IUnitsManager
    {

        public UnitsManager(DbContexts contexts) : base(contexts)
        {
        }

        public async Task<List<UnitsDTO>> SearchUnitsAsync(UnitSearchRequest request, UserSessionDTO User)
        {
            // مستقیم توی خود متد
            var baseUrl = "http://87.107.111.44:8010";
            var token = User.AccessToken.ToString();

            var bodyObj = new
            {
                page = request.Page,
                pageSize = request.PageSize,
                filters = new[]
                {
                new
                {
                    field = "",
                    @operator = "Equals",
                    value = (string)null
                }
            },
                sortBy = (string)null,
                sortDescending = true
            };

            var json = JsonSerializer.Serialize(
                bodyObj,
                new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

            using var client = new HttpClient();

            var httpRequest = new HttpRequestMessage(
                HttpMethod.Post,
                $"{baseUrl}/api/admin/units/search");

            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            httpRequest.Content = new StringContent(json, Encoding.UTF8, "application/json");

            using var response = await client.SendAsync(httpRequest);
            response.EnsureSuccessStatusCode();

            var stream = await response.Content.ReadAsStreamAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var result = await JsonSerializer.DeserializeAsync<ApiResponse<List<UnitsDTO>>>(stream, options);

            return result?.Data ?? new List<UnitsDTO>();
        }



    }
}
