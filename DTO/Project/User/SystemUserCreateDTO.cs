using DTO.Project.SecurityQuestion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Project.User
{
    public class SystemUserCreateDTO
    {
        public string UnitId { get; set; }              // unitId
        public string UserName { get; set; }           // userName
        public string InitialPassword { get; set; }    // initialPassword
        public string FirstName { get; set; }          // firstName
        public string LastName { get; set; }           // lastName
        public string NationalCode { get; set; }       // nationalCode
        public string MobileNumber { get; set; }       // mobileNumber

        public List<SecurityQuestionCreateDTO> SecurityQuestions { get; set; }
            = new List<SecurityQuestionCreateDTO>();  // securityQuestions

        public List<Guid> RoleIds { get; set; } = new(); // roleIds
    }
}
