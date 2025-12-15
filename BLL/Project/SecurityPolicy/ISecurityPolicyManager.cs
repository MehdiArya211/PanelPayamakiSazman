using DTO.Base;
using DTO.Project.SecurityPolicy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Project.SecurityPolicy
{
    public interface ISecurityPolicyManager
    {
        SecurityPolicyDTO Get();
        BaseResult Update(SecurityPolicyDTO model);
    }
}
