using DTO.Base;
using DTO.Project.SecurityPolicy;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace BLL.Project.SecurityPolicy
{
    public class SecurityPolicyManager : ISecurityPolicyManager
    {
        private readonly IHttpContextAccessor _httpContext;
        private readonly HttpClient _client;
        private readonly string _baseUrl;

        public SecurityPolicyManager(IHttpContextAccessor accessor, IConfiguration config)
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

        // ================= GET =================
        public SecurityPolicyDTO Get()
        {
            SetAuth();

            var res = _client.GetAsync($"{_baseUrl}/security-policy").Result;
            if (!res.IsSuccessStatusCode) return null;

            var json = res.Content.ReadAsStringAsync().Result;

            var api = JsonSerializer.Deserialize<SecurityPolicyApiResponseDTO>(
                json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );

            var d = api?.Data;
            if (d == null) return null;

            return new SecurityPolicyDTO
            {
                PasswordMinLength = d.PasswordPolicy.MinLength,
                RequireUppercase = d.PasswordPolicy.RequireUppercase,
                RequireLowercase = d.PasswordPolicy.RequireLowercase,
                RequireDigit = d.PasswordPolicy.RequireDigit,
                RequireSpecial = d.PasswordPolicy.RequireSpecial,
                MinimumDifferentCharacters = d.PasswordPolicy.MinimumDifferentCharacters,
                RotationEnabled = d.PasswordPolicy.RotationEnabled,
                RotationDays = d.PasswordPolicy.RotationDays,
                PasswordHistoryLimit = d.PasswordPolicy.PasswordHistoryLimit,

                MaxFailedAttempts = d.LockoutPolicy.MaxFailedAttempts,
                LockoutWindowMinutes = d.LockoutPolicy.LockoutWindowMinutes,
                LockoutDurationMinutes = d.LockoutPolicy.LockoutDurationMinutes,

                AccessTokenMinutes = d.TokenPolicy.AccessTokenMinutes,
                RefreshTokenDays = d.TokenPolicy.RefreshTokenDays,

                ReAuthenticationMinutes = d.ReAuthenticationPolicy.FreshnessMinutes
            };
        }

        // ================= UPDATE =================
        public BaseResult Update(SecurityPolicyDTO model)
        {
            try
            {
                SetAuth();

                var body = new
                {
                    passwordPolicy = new
                    {
                        minLength = model.PasswordMinLength,
                        requireUppercase = model.RequireUppercase,
                        requireLowercase = model.RequireLowercase,
                        requireDigit = model.RequireDigit,
                        requireSpecial = model.RequireSpecial,
                        minimumDifferentCharacters = model.MinimumDifferentCharacters,
                        rotationEnabled = model.RotationEnabled,
                        rotationDays = model.RotationDays,
                        passwordHistoryLimit = model.PasswordHistoryLimit
                    },
                    lockoutPolicy = new
                    {
                        maxFailedAttempts = model.MaxFailedAttempts,
                        lockoutWindowMinutes = model.LockoutWindowMinutes,
                        lockoutDurationMinutes = model.LockoutDurationMinutes
                    },
                    tokenPolicy = new
                    {
                        accessTokenMinutes = model.AccessTokenMinutes,
                        refreshTokenDays = model.RefreshTokenDays
                    },
                    reAuthenticationPolicy = new
                    {
                        freshnessMinutes = model.ReAuthenticationMinutes
                    }
                };

                var json = JsonSerializer.Serialize(body);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var res = _client.PutAsync($"{_baseUrl}/security-policy", content).Result;

                return res.IsSuccessStatusCode
                    ? new BaseResult(true, "سیاست‌های امنیتی با موفقیت ذخیره شد.")
                    : new BaseResult(false, res.Content.ReadAsStringAsync().Result);
            }
            catch (Exception ex)
            {
                return new BaseResult(false, ex.Message);
            }
        }
    }
}
