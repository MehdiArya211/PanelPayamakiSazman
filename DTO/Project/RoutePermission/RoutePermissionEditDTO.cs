using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Project.RoutePermission
{
    public class RoutePermissionEditDTO
    {
        [Required(ErrorMessage = "Route Key الزامی است")]
        public string RouteKey { get; set; }

        [Required(ErrorMessage = "حداقل یک Action باید انتخاب شود")]
        [MinLength(1, ErrorMessage = "حداقل یک Action باید انتخاب شود")]
        public List<string> Actions { get; set; } = new List<string>();

        [Required(ErrorMessage = "وضعیت RequiresFreshAuth الزامی است")]
        public bool RequiresFreshAuth { get; set; }

        // برای نمایش در UI
        public string DisplayRouteKey => RouteKey?.Replace('.', '\u2192');
    }
}
