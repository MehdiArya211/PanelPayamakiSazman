

using DTO.Project.SystemMenu;
using System.ComponentModel.DataAnnotations;

namespace DTO.Project.User
{
    /// <summary>
    /// اطلاعات کاربر که پس از لاگین در Session نگهداری می‌شود
    /// (کاملاً WebService-based)
    /// </summary>
    public class SystemUserSessionDTO 
    {
        public string Id { get; set; }
        [Display(Name = "نام کاربری")]
        public string Username { get; set; }

        [Display(Name = "تلفن همراه")]
        public string Mobile { get; set; }

        [Display(Name = "نام کامل")]
        public string FullName { get; set; }

        [Display(Name = "فعال است")]
        public bool IsEnabled { get; set; }

        /// <summary>
        /// نقش کاربر (صرفاً نمایشی – تصمیم‌گیر API است)
        /// </summary>
        public string Role { get; set; }

        //[Display(Name = "نوع کاربر")]
       // public UserType? Type { get; set; }

        /// <summary>
        /// دوره تغییر رمز عبور
        /// </summary>
        public int? ChangePasswordCycle { get; set; }

        /// <summary>
        /// منوهای مجاز کاربر (دریافتی از Web API)
        /// </summary>
        public List<SystemMenuSessionDTO> Menus { get; set; } = new();

        #region Token Info

        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime AccessTokenExpiresAt { get; set; }
        public DateTime RefreshTokenExpiresAt { get; set; }
        public Guid SessionId { get; set; }

        #endregion

        public bool PasswordIsChanged { get; set; }
        public bool PasswordExpired { get; set; }

        #region Helper Methods (UI Level)

        /// <summary>
        /// آیا کاربر به منوی خاصی دسترسی دارد؟
        /// </summary>
        public bool HasMenu(string menuId)
        {
            return Menus.Any(x => x.Id == menuId);
        }

        /// <summary>
        /// بررسی دسترسی بر اساس Area / Controller / Action
        /// </summary>
        public bool HasMenu(string area, string controller, string action)
        {
            area = area?.Trim().ToLower();
            controller = controller?.Trim().ToLower();
            action = action?.Trim().ToLower();

            return Menus.Any(x =>
                x.Area?.ToLower() == area &&
                x.Controller?.ToLower() == controller &&
                x.Action?.ToLower() == action);
        }

        #endregion
    }
}

