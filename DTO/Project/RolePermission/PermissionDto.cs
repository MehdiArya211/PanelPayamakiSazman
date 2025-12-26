using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DTO.Project.RolePermission
{
    /// <summary>
    /// مدل نمایش مجوز دسترسی (از API دریافت می‌شود)
    /// </summary>
    public class PermissionDto
    {
        [JsonPropertyName("routeKey")]
        public string RouteKey { get; set; }

        [JsonPropertyName("availableActions")]
        public List<string> AvailableActions { get; set; } = new List<string>();

        [JsonPropertyName("assignedActions")]
        public List<string> AssignedActions { get; set; } = new List<string>();

        // برای نمایش در UI
        public string DisplayRouteKey => RouteKey?.Replace('.', '\u2192');
        public string PersianRouteName => RouteNameMapping.GetPersianName(RouteKey);
        public bool HasAnyAssigned => AssignedActions != null && AssignedActions.Count > 0;
        public int AssignedCount => AssignedActions?.Count ?? 0;
        public int AvailableCount => AvailableActions?.Count ?? 0;

        // برای نمایش در UI - ترکیب نام فارسی و Route Key
        public string DisplayName => $"{PersianRouteName} <small class='text-muted'>({DisplayRouteKey})</small>";
    }
}
