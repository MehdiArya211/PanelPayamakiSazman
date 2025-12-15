using System.ComponentModel.DataAnnotations;

namespace DTO.Project.SecurityPolicy
{
    public class SecurityPolicyDTO
    {
        // ===== سیاست رمز عبور =====
        [Display(Name = "حداقل طول رمز عبور")]
        public int PasswordMinLength { get; set; }

        [Display(Name = "الزام حروف بزرگ (A-Z)")]
        public bool RequireUppercase { get; set; }

        [Display(Name = "الزام حروف کوچک (a-z)")]
        public bool RequireLowercase { get; set; }

        [Display(Name = "الزام استفاده از عدد")]
        public bool RequireDigit { get; set; }

        [Display(Name = "الزام کاراکتر خاص (@,#,$ و ...)")]
        public bool RequireSpecial { get; set; }

        [Display(Name = "حداقل تعداد کاراکتر متفاوت")]
        public int MinimumDifferentCharacters { get; set; }

        // ===== چرخش و تاریخچه رمز عبور =====
        [Display(Name = "فعال‌سازی تغییر دوره‌ای رمز عبور")]
        public bool RotationEnabled { get; set; }

        [Display(Name = "مدت اعتبار رمز عبور (روز)")]
        public int RotationDays { get; set; }

        [Display(Name = "تعداد رمزهای قبلی غیرقابل استفاده")]
        public int PasswordHistoryLimit { get; set; }

        // ===== قفل شدن حساب =====
        [Display(Name = "حداکثر تلاش ناموفق ورود")]
        public int MaxFailedAttempts { get; set; }

        [Display(Name = "بازه زمانی بررسی تلاش ناموفق (دقیقه)")]
        public int LockoutWindowMinutes { get; set; }

        [Display(Name = "مدت زمان قفل بودن حساب (دقیقه)")]
        public int LockoutDurationMinutes { get; set; }

        // ===== توکن و احراز هویت =====
        [Display(Name = "مدت اعتبار Access Token (دقیقه)")]
        public int AccessTokenMinutes { get; set; }

        [Display(Name = "مدت اعتبار Refresh Token (روز)")]
        public int RefreshTokenDays { get; set; }

        [Display(Name = "مدت زمان نیاز به احراز هویت مجدد (دقیقه)")]
        public int ReAuthenticationMinutes { get; set; }
    }
}
