using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Project.RolePermission
{
    public static class RouteNameMapping
    {
        private static readonly Dictionary<string, string> PersianNames = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            // Authentication Routes
            { "Auth.Login", "ورود به سیستم" },
            { "Auth.Register", "ثبت‌نام" },
            { "Auth.Logout", "خروج از سیستم" },
            { "Auth.ForgotPassword", "فراموشی رمز عبور" },
            { "Auth.ResetPassword", "بازنشانی رمز عبور" },
            { "Auth.ChangePassword", "تغییر رمز عبور" },
            { "Auth.VerifyEmail", "تایید ایمیل" },
            { "Auth.TwoFactor", "احراز دو مرحله‌ای" },
            
            // User Management
            { "Users.Index", "مدیریت کاربران" },
            { "Users.Create", "ایجاد کاربر" },
            { "Users.Edit", "ویرایش کاربر" },
            { "Users.Delete", "حذف کاربر" },
            { "Users.Details", "جزئیات کاربر" },
            { "Users.Profile", "پروفایل کاربر" },
            { "Users.ChangeStatus", "تغییر وضعیت کاربر" },
            
            // Role Management
            { "Roles.Index", "مدیریت نقش‌ها" },
            { "Roles.Create", "ایجاد نقش" },
            { "Roles.Edit", "ویرایش نقش" },
            { "Roles.Delete", "حذف نقش" },
            { "Roles.AssignPermissions", "اختصاص مجوز" },
            
            // Permission Management
            { "Permissions.Index", "مدیریت مجوزها" },
            { "Permissions.Edit", "ویرایش مجوز" },
            
            // Dashboard
            { "Dashboard.Index", "داشبورد" },
            { "Dashboard.Analytics", "آمار و تحلیل" },
            { "Dashboard.Reports", "گزارشات" },
            
            // Settings
            { "Settings.General", "تنظیمات عمومی" },
            { "Settings.Email", "تنظیمات ایمیل" },
            { "Settings.SMS", "تنظیمات پیامک" },
            { "Settings.Notification", "تنظیمات اطلاع‌رسانی" },
            
            // Logs
            { "Logs.Audit", "لاگ عملیات" },
            { "Logs.Error", "لاگ خطاها" },
            { "Logs.LoginAttempts", "لاگ ورود به سیستم" },
            { "Logs.Activity", "لاگ فعالیت‌ها" },
            
            // File Management
            { "Files.Upload", "آپلود فایل" },
            { "Files.Download", "دانلود فایل" },
            { "Files.Delete", "حذف فایل" },
            
            // Notifications
            { "Notifications.Index", "اعلان‌ها" },
            { "Notifications.MarkAsRead", "علامت‌گذاری خوانده شده" },
            { "Notifications.ClearAll", "پاک کردن همه" },
            
            // Profile
            { "Profile.Index", "پروفایل" },
            { "Profile.Edit", "ویرایش پروفایل" },
            { "Profile.Security", "امنیت" },
            
            // API
            { "Api.Documentation", "مستندات API" },
            { "Api.Test", "تست API" },
            
            // Common
            { "Common.Export", "خروجی گرفتن" },
            { "Common.Import", "ورود اطلاعات" },
            { "Common.Search", "جستجو" },
            { "Common.Filter", "فیلتر" },
            { "Common.Sort", "مرتب‌سازی" }
        };

        public static string GetPersianName(string routeKey)
        {
            if (string.IsNullOrWhiteSpace(routeKey))
                return routeKey;

            // Try exact match first
            if (PersianNames.ContainsKey(routeKey))
                return PersianNames[routeKey];

            // Try partial match (for nested routes)
            foreach (var key in PersianNames.Keys)
            {
                if (routeKey.StartsWith(key, StringComparison.OrdinalIgnoreCase))
                {
                    var remaining = routeKey.Substring(key.Length).TrimStart('.');
                    var persianName = PersianNames[key];

                    if (!string.IsNullOrWhiteSpace(remaining))
                    {
                        return $"{persianName} → {remaining}";
                    }
                    return persianName;
                }
            }

            // If no match found, return the route key with proper formatting
            return FormatRouteKey(routeKey);
        }

        private static string FormatRouteKey(string routeKey)
        {
            if (string.IsNullOrWhiteSpace(routeKey))
                return routeKey;

            // Split by dots and capitalize first letter of each part
            var parts = routeKey.Split('.');
            var formattedParts = parts.Select(p =>
            {
                if (string.IsNullOrWhiteSpace(p))
                    return p;

                // Convert camelCase to Title Case
                var result = System.Text.RegularExpressions.Regex.Replace(p, "([a-z])([A-Z])", "$1 $2");
                return char.ToUpper(result[0]) + result.Substring(1);
            });

            return string.Join(" → ", formattedParts);
        }
    }
}
