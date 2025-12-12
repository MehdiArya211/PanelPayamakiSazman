using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Project.SenderNumberAssignment
{
    /// <summary>
    /// DTO نمایش تخصیص سرشماره به کاربر
    /// </summary>
    public class SenderNumberAssignmentListDTO
    {
        public string Id { get; set; }

        public string SenderNumberId { get; set; }
        public string FullNumber { get; set; }
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

        public string SenderNumberStatus { get; set; }   // Purchasing / Active / ...

        public string UserId { get; set; }
        public string UserName { get; set; }
        public string UserFullName { get; set; }
        public bool IsServiceClient { get; set; }

        /// <summary>
        /// وضعیت تخصیص: Active / Revoked (یا شبیه این‌ها)
        /// </summary>
        public string Status { get; set; }

        public DateTime? AssignedAt { get; set; }
        public string AssignedByUserId { get; set; }
        public string AssignedByUserName { get; set; }

        public DateTime? RevokedAt { get; set; }
        public string RevokedByUserId { get; set; }
        public string RevokedByUserName { get; set; }
        public string RevocationReason { get; set; }

        public string Description { get; set; }
    }
}
