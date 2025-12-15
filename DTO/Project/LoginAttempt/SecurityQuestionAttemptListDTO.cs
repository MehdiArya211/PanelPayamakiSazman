using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Project.LoginAttempt
{
    public class SecurityQuestionAttemptListDTO
    {
        public string Username { get; set; }
        public bool IsSuccessful { get; set; }
        public int AttemptCount { get; set; }
        public string IpAddress { get; set; }
        public DateTime AttemptDate { get; set; }
    }

}
