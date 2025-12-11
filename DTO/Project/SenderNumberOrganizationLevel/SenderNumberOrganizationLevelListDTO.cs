using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Project.SenderNumberOrganizationLevel
{
    /// <summary>
    /// رده‌های سازمانی سرشماره (لیست)
    /// </summary>
    public class SenderNumberOrganizationLevelListDTO
    {
        public string Id { get; set; }          // API می‌گه uuid، نمونه‌اش int نشون داده؛ ما string می‌گیریم
        public string Code { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
    }
}
