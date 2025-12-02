using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.WebApi
{
    public class LoginApiResultDTO
    {
        public string accessToken { get; set; }
        public string refreshToken { get; set; }
        public DateTime accessTokenExpiresAt { get; set; }
        public DateTime refreshTokenExpiresAt { get; set; }
        public Guid sessionId { get; set; }
        public bool mustChangePassword { get; set; }
        public bool passwordExpired { get; set; }
    }
}
