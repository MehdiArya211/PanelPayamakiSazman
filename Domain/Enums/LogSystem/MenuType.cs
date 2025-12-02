using System.ComponentModel;

namespace Domain.Enums
{
    /// <summary>
    /// نوع منو
    /// </summary>
    public enum MenuType
    {
        [Description("ورود کاربر")]
        Login = 0,

        [Description("پروفایل کاربر")]
        Profile = 1,

        [Description("خطای دسترسی")]
        ErrorPage = 2,

        [Description("منو ها")]
        Menus = 3,

        [Description("نقش ها")]
        Roles = 4,

        [Description("پرسنل")]
        Users = 5,

        [Description("لاگ فعالیت")]
        ActionLog = 6,

        [Description("لاگ لاگین")]
        LoginLog = 7,

        [Description("تنظیمات")]
        Constants = 8,

        [Description("فیلتر احراز هویت")]
        AuthorizeFilter = 9,



    }
}
