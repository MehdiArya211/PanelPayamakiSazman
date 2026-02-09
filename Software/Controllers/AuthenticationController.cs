using BLL.Interface;
using BLL.Project.SystemMenu;
using DocumentFormat.OpenXml.Math;
using DTO.Project.User;
using DTO.User;
using FajrLog.Enum;
using Microsoft.AspNetCore.Mvc;
using Services.CookieServices;
using Services.RedisService;
using Services.SessionServices;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using Utilities.Extentions;


namespace Momayezi.Controllers
{
    /// <summary>
    /// صفحه لاگین کاربران با یوزر پس
    /// </summary>
    public class AuthenticationController : Controller
    {
        private readonly IAuthManager AuthManager;
        private readonly ISystemMenuManager SystemMenuManager;
        private readonly IUserLogManager UserLogManager;
        private readonly IConstantManager ConstantManager;
        private readonly IRedisManager Redis;
        private readonly ISession Session;

        public AuthenticationController(IAuthManager _AuthManager, IUserLogManager _UserLogManager, IRedisManager _Redis, IConstantManager constantManager, ISystemMenuManager _systemMenuManager) : base()
        {
            AuthManager = _AuthManager;
            UserLogManager = _UserLogManager;
            Redis = _Redis;
            Session = Redis.ContextAccessor.HttpContext.Session;
            ConstantManager = constantManager;
            SystemMenuManager = _systemMenuManager;
        }


        #region لاگین به پنل کاربر
        /// <summary>
        /// لاگین
        /// </summary>
        public IActionResult Index(long? mid)
        {
            if (mid == null)
            {
                var User = Session.GetUser();
                if (User != null)
                    return RedirectToAction("index", "Dashboard", new { area = "Admin" });
            }

            var showCaptcha = HttpContext.GetCookieShowCaptcha();
            if (showCaptcha)
                ViewBag.captcha = true;

            return View();
        }


        public IActionResult Login()
        {

            var showCaptcha = HttpContext.GetCookieShowCaptcha();
            if (showCaptcha)
                ViewBag.captcha = true;

            return View("index");
        }



        [HttpPost]
        public async Task<IActionResult> Index0(string Mobile, string Password, string Captcha, string RetUrl, int? MenuId)
        {
            FajrActionType fajrActionType = MenuId == null ? FajrActionType.logIn : FajrActionType.logInSecurePage;

            var res = await AuthManager.Login(Mobile, Password);
            var menus =  SystemMenuManager.GetMenusForCurrentUser();

            var user = res.Model as SystemUserSessionDTO;

            // ❗ اگر رمز منقضی یا نیاز به تغییر دارد
            if (user != null && (user.PasswordExpired || user.PasswordIsChanged))
            {
                // ذخیره اطلاعات لازم برای تغییر رمز
                HttpContext.Session.SetString("ChangePass_Username", Mobile);
                HttpContext.Session.SetString("ChangePass_CurrentPassword", Password);
                HttpContext.Session.SetString("ChangePass_Token", user.AccessToken);

                return RedirectToAction("ForceChangePassword");
            }

            // ادامه همان کد شما…
            if (!res.Status || user == null)
            {
                ViewBag.Error = res.Message;
                ViewBag.InvalidLogin = true;
                return View();
            }

            HttpContext.Session.SetUser(user);
            return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
        }

        [HttpPost]
        public async Task<IActionResult> Index(
    string Mobile,
    string Password,
    string Captcha,
    string RetUrl,
    int? MenuId)
        {
            FajrActionType fajrActionType =
                MenuId == null ? FajrActionType.logIn : FajrActionType.logInSecurePage;

            // 1️⃣ لاگین
            var res = await AuthManager.Login(Mobile, Password);
            var user = res.Model as SystemUserSessionDTO;

            // 2️⃣ بررسی خطا
            if (!res.Status || user == null)
            {
                ViewBag.Error = res.Message;
                ViewBag.InvalidLogin = true;
                return View();
            }

            // 3️⃣ بررسی تغییر رمز
            if (user.PasswordExpired || user.PasswordIsChanged)
            {
                HttpContext.Session.SetString("ChangePass_Username", Mobile);
                HttpContext.Session.SetString("ChangePass_CurrentPassword", Password);
                HttpContext.Session.SetString("ChangePass_Token", user.AccessToken);

                return RedirectToAction("ForceChangePassword");
            }

            // 4️⃣ ذخیره Token در Session (خیلی مهم)
            HttpContext.Session.SetString("AccessToken", user.AccessToken);
            HttpContext.Session.SetString("RefreshToken", user.RefreshToken);

            // 5️⃣ گرفتن منو با Token
            var menus = await SystemMenuManager.GetMenusForCurrentUser();

            // 6️⃣ اتصال منو به کاربر
            user.Menus = menus;

            // 7️⃣ ذخیره کاربر کامل در Session
            HttpContext.Session.SetUser(user);

            // 8️⃣ رفتن به داشبورد
            return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
        }



        public IActionResult ForceChangePassword()
        {
            var token = HttpContext.Session.GetString("ChangePass_Token");

            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Index");

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ForceChangePassword(string NewPassword)
        {
            var username = HttpContext.Session.GetString("ChangePass_Username");
            var oldPassword = HttpContext.Session.GetString("ChangePass_CurrentPassword");
            var token = HttpContext.Session.GetString("ChangePass_Token");

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(oldPassword) || string.IsNullOrEmpty(token))
            {
                TempData["Error"] = "اطلاعات سشن یافت نشد.";
                return RedirectToAction("Index");
            }

            using var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post,
                "http://samp.core.api:80/api/auth/change-password");

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var body = new
            {
                userName = username,
                currentPassword = oldPassword,
                newPassword = NewPassword
            };

            request.Content = new StringContent(
                System.Text.Json.JsonSerializer.Serialize(body),
                Encoding.UTF8,
                "application/json");

            var response = await client.SendAsync(request);
            var result = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "خطا در تغییر رمز عبور: " + result;
                return RedirectToAction("ForceChangePassword");
            }

            // ✔ لاگین جدید با رمز جدید
            var login = await AuthManager.Login(username, NewPassword);

            if (!login.Status)
            {
                TempData["Error"] = "رمز تغییر کرد، اما ورود مجدد ناموفق بود.";
                return RedirectToAction("Index");
            }

            HttpContext.Session.SetUser(login.Model as SystemUserSessionDTO);

            // پاک‌کردن سشن‌ها
            HttpContext.Session.Remove("ChangePass_Username");
            HttpContext.Session.Remove("ChangePass_CurrentPassword");
            HttpContext.Session.Remove("ChangePass_Token");

            return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
        }



        #endregion


        #region خروج از حساب کاربری - logout
        public async Task<ActionResult> Logout()
        {
            var user = Session.GetUser();

            if (user != null)
            {
                // 1) فراخوانی وب‌سرویس logout
                try
                {
                    using var client = new HttpClient();

                    var request = new HttpRequestMessage(HttpMethod.Post,
                        "http://samp.core.api:80/api/auth/logout");

                    // 🔹 استفاده از AccessToken کاربر جاری
                    request.Headers.Authorization =
                        new AuthenticationHeaderValue("Bearer", user.AccessToken);

                    using var response = await client.SendAsync(request);

                    // (اختیاری) اگر لازم داری خروجی سرویس را بخوانی:
                    // var content = await response.Content.ReadAsStringAsync();
                }
                catch (Exception ex)
                {
                    // لاگ خطا در صورت نیاز
                }

                // 2) ثبت لاگ خروج
                _ = Redis.db.SetLoginLog(
                    Redis.ContextAccessor,
                    FajrActionType.logOut,
                    user.Username,
                    user.FullName,
                    user.Id,
                    true,
                    $"کاربر {user.FullName} خارج شد.",
                    true
                ).Result;

                // 3) حذف سشن
                Session.RemoveUser();

                // 4) حذف کوکی
                HttpContext.Response.Cookies.Delete("_Session.cookie");
            }

            return RedirectToAction("Index");
        }

        #endregion


    }
}