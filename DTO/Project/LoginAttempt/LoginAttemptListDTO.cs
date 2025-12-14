using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Project.LoginAttempt
{
    public class LoginAttemptListDTO
    {
        public string Username { get; set; }
        public bool IsSuccessful { get; set; }
        public string IpAddress { get; set; }
        public DateTime AttemptDate { get; set; }
        public string FailureReason { get; set; }
    }
}
