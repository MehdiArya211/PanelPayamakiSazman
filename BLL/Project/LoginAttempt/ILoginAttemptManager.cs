using DTO.DataTable;
using DTO.Project.LoginAttempt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Project.LoginAttempt
{
    public interface ILoginAttemptManager
    {
        public DataTableResponseDTO<LoginAttemptListDTO> GetLoginAttemptsDataTable(DataTableSearchDTO search);
        public DataTableResponseDTO<SecurityQuestionAttemptListDTO> GetSecurityQuestionAttemptsDataTable(DataTableSearchDTO search);


    }
}
