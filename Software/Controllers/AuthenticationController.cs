using BLL.Interface;
using DTO.User;
using FajrLog.Enum;
using Microsoft.AspNetCore.Mvc;
using Services.CookieServices;
using Services.RedisService;
using Services.SessionServices;
using System.Net.Http.Headers;
using System.Reflection;
using Utilities.Extentions;


namespace Momayezi.Controllers
{
    /// <summary>
    /// صفحه لاگین کاربران با یوزر پس
    /// </summary>
    public class AuthenticationController : Controller
    {
        private readonly IAuthManager AuthManager;
        private readonly IUserLogManager UserLogManager;
        private readonly IConstantManager ConstantManager;
        private readonly IRedisManager Redis;
        private readonly ISession Session;

        public AuthenticationController(IAuthManager _AuthManager, IUserLogManager _UserLogManager, IRedisManager _Redis, IConstantManager constantManager) : base()
        {
            AuthManager = _AuthManager;
            UserLogManager = _UserLogManager;
            Redis = _Redis;
            Session = Redis.ContextAccessor.HttpContext.Session;
            ConstantManager = constantManager;
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




        /// <summary>
        /// لاگین با یوزر پس
        /// </summary>
        /// <param name="Mobile">نام کاربری</param>
        /// <param name="Password">پسورد</param>
        /// <param name="MenuId">اگر لاگین جهت احراز هویت مجدد برای منو خاص بود مقدار دارد</param>
        /// <param name="RetUrl"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Index(string Mobile, string Password, string Captcha, string RetUrl, long? MenuId)
        {
            FajrActionType fajrActionType = MenuId == null ? FajrActionType.logIn : FajrActionType.logInSecurePage;

            #region بررسی تعداد تلاش برای لاگین و قفل بودن حساب کاربری
            //لاگ دفعات تلاش برای لاگین
            var loginLog = await Redis.db.SetLoginLog(Mobile);
            var FailedLoginCount = ConstantManager.GetFailedLoginCount();
            if (loginLog != null && loginLog.Count > FailedLoginCount)
            {
                fajrActionType = FajrActionType.BlockUser;
                var diff = (int)(loginLog.CreateDate.AddMinutes(20) - DateTime.Now).TotalMinutes;
                await Redis.db.SetLoginLog(Redis.ContextAccessor, fajrActionType, Mobile, null, null, false, $"مسدود شدن حساب کاربری تا {diff} دقیقه دیگر به دلیل {loginLog.Count} بار ورود اشتباه کلمه عبور. ");
                ViewBag.Error = $"حساب کاربری شما بدلیل ورود اشتباه کلمه عبور تا {diff} دقیقه آینده مسدود می باشد.";
                return View();
            }
            #endregion

            #region بررسی کپچا در صورت وجود
            // برای دفعات بعدی لاگین در صورت اشتباه وارد کردن کلمه عبور، کپچا نشان داده شود
            ViewBag.captcha = true;
            var showCaptcha = HttpContext.GetCookieShowCaptcha();
            HttpContext.SetCookieShowCaptcha();

            if (showCaptcha)
            {
                var captcha = HttpContext.Session.GetString("Captcha")?.Trim().ToEnglishNumber();
                if (string.IsNullOrEmpty(captcha) || captcha != Captcha)
                {
                    await Redis.db.SetLoginLog(Redis.ContextAccessor, fajrActionType, Mobile, null, null, false, "کد امنیتی صحیح نیست!");
                    ViewBag.Error = "کد امنیتی صحیح نیست!";
                    return View();
                }
            }
            #endregion

            #region عملیات لاگین
            var res =await AuthManager.Login(Mobile, Password);
            var User = res.Model as UserSessionDTO;
            if (!res.Status)
            {
                await Redis.db.SetLoginLog(Redis.ContextAccessor, fajrActionType, Mobile, User?.FullName, User?.Id, false, res.Message);
                if (User == null)
                {
                    ViewBag.Error = res.Message;
                    ViewBag.InvalidLogin = true;

                    return View();
                }
            }
            #endregion

            #region اطلاعات ردیس و کوکی
            // افزودن لاگ برای لاگین موفق
            await Redis.db.SetLoginLog(Redis.ContextAccessor, fajrActionType, Mobile, User?.FullName, User?.Id, true, $"کاربر {User.FullName} وارد شد.");

            if (MenuId == null)
            {
                // افزودن توکن کاربر به ردیس برای جلوگیری از لاگین همزمان 2 نفر با یک اکانت
                var token = await Redis.db.SetLoginToken(User.Id);

                // افزودن توکن به کوکی
                HttpContext.SetCookieUserToken(token);
            }
            else
            {
                // اگر لاگین جهت احراز هویت مجدد برای منو خاص بود، اطلاعات درون ردیس ذخیره شود
                await Redis.db.SetUserReAuthorizeMenu((int)MenuId, User.Id);
            }
            // حذف اطلاعات مربوط به کنترل تعداد دفعات تلاش برای لاگین
            await Redis.db.RemoveLoginLog(Mobile);

            // حذف اطلاعات نمایش کپچا به کاربر
            HttpContext.RemoveCookieShowCaptcha();
            #endregion

            if (!string.IsNullOrEmpty(RetUrl))
                return Redirect(RetUrl);

            return RedirectToAction("index", "Dashboard", new { area = "Admin" });
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
                        "http://87.107.111.44:8010/api/auth/logout");

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