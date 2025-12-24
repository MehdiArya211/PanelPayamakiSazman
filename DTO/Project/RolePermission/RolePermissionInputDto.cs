using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Project.RolePermission
{
    /// <summary>
    /// مدل ورودی برای ارسال مجوزهای یک Route
    /// </summary>
    public class RolePermissionInputDto
    {
        [Required(ErrorMessage = "Route Key الزامی است")]
        public string RouteKey { get; set; }

        public List<string> Actions { get; set; } = new List<string>();
    }
}
