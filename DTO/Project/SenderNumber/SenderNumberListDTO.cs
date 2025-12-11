    using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Project.SenderNumber
{
    /// <summary>
    /// مدل نمایشی سرشماره در لیست‌ها
    /// </summary>
    public class SenderNumberListDTO
    {
        public string Id { get; set; }                 
        public string FixedPrefix { get; set; }         
        public string SpecialtyId { get; set; }         
        public string SpecialtyCode { get; set; }
        public string SpecialtyTitle { get; set; }
        public string OrganizationLevelId { get; set; }
        public string OrganizationLevelCode { get; set; }
        public string OrganizationLevelTitle { get; set; }
        public string SubAreaId { get; set; }
        public string SubAreaCode { get; set; }
        public string SubAreaTitle { get; set; }
        public string FullNumber { get; set; }
        public string Status { get; set; }              // Purchasing, Active, ...
        public string? Description { get; set; }
    }
}
