using System.Text.Json.Serialization;

namespace DTO.WebApi
{
    public class LoginApiResultDTO
    {
        [JsonPropertyName("accessToken")]
        public string AccessToken { get; set; }

        [JsonPropertyName("refreshToken")]
        public string RefreshToken { get; set; }

        [JsonPropertyName("accessTokenExpiresAt")]
        public DateTime AccessTokenExpiresAt { get; set; }

        [JsonPropertyName("refreshTokenExpiresAt")]
        public DateTime RefreshTokenExpiresAt { get; set; }

        // JSON: "sessionId": "019af3bc-e4dd-77ac-8b87-0dc9212864da"
        [JsonPropertyName("sessionId")]
        public Guid SessionId { get; set; }

        [JsonPropertyName("mustChangePassword")]
        public bool MustChangePassword { get; set; }

        [JsonPropertyName("passwordExpired")]
        public bool PasswordExpired { get; set; }
    }
    public class ApiResponse<T>
    {
        [JsonPropertyName("data")]
        public T Data { get; set; }

        [JsonPropertyName("traceId")]
        public string TraceId { get; set; }
    }
}
