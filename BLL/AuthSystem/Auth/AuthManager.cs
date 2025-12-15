using BLL.Interface;
using Domain.Entities;
using Domain.Enums;
using DTO;
using DTO.Base;
using DTO.Menu;
using DTO.Project.User;
using DTO.User;
using DTO.WebApi;
using Infrastructure.Data;
using LinqKit;
using Microsoft.AspNetCore.Http;
using Services.SessionServices;
using System.Net.Http.Headers;
using System.Text;
using Utilities;
using Utilities.Extentions;

namespace BLL
{


    /// <summary>
    /// عملیات احراز هویت و لاگین
    /// </summary>
    public class AuthManager : Manager<User, ApplicationContext>, IAuthManager
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly ISession Session;

        public AuthManager(DbContexts _Context, IHttpContextAccessor _httpContextAccessor) : base(_Context)
        {
            httpContextAccessor = _httpContextAccessor;
            Session = httpContextAccessor.HttpContext.Session;
        }


        /// <summary>
        /// گرفتن اطلاعات لاگین کاربراز سرویس
        /// </summary>
        /// <param name="Username">نام کاربری</param>
        /// <param name="Password">کلمه عبور</param>
        /// <returns></returns>

        public async Task<BaseResult> Login(string Username = null, string Password = null)
        {
            Username = Username?.Trim();
            Password = Password?.Trim();

            if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Password))
                return new BaseResult
                {
                    Status = false,
                    Message = "نام کاربری و کلمه عبور را وارد کنید"
                };

            try
            {
                using var client = new HttpClient();

                var request = new HttpRequestMessage(HttpMethod.Post,
                    "http://87.107.111.44:8010/api/auth/login");

                var body = new
                {
                    userName = Username,
                    password = Password
                };

                request.Content = new StringContent(
                    System.Text.Json.JsonSerializer.Serialize(body),
                    Encoding.UTF8,
                    "application/json");

                using var response = await client.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    var errorText = await response.Content.ReadAsStringAsync();
                    return new BaseResult
                    {
                        Status = false,
                        Message = "نام کاربری یا کلمه عبور اشتباه است",
                        Model = errorText
                    };
                }

                string json = await response.Content.ReadAsStringAsync();

                var options = new System.Text.Json.JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var apiResponse = System.Text.Json.JsonSerializer
                    .Deserialize<DTO.WebApi.ApiResponse<LoginApiResultDTO>>(json, options);

                if (apiResponse?.Data == null)
                {
                    return new BaseResult
                    {
                        Status = false,
                        Message = "پاسخی از وب‌سرویس دریافت نشد"
                    };
                }

                var apiResult = apiResponse.Data;

                var user = new SystemUserSessionDTO
                {
                    AccessToken = apiResult.AccessToken,
                    RefreshToken = apiResult.RefreshToken,
                    AccessTokenExpiresAt = apiResult.AccessTokenExpiresAt,
                    RefreshTokenExpiresAt = apiResult.RefreshTokenExpiresAt,
                    SessionId = apiResult.SessionId,
                    PasswordIsChanged = apiResult.MustChangePassword,
                    PasswordExpired = apiResult.PasswordExpired,
                    Username = Username,
                    FullName = Username
                };

                Session.SetString("AdminToken", apiResult.AccessToken);

                var jsonUser = System.Text.Json.JsonSerializer.Serialize(user);
                Session.SetString("UserSessionRaw", jsonUser);
                Session.SetUser(user);

                return new BaseResult
                {
                    Status = true,
                    Message = "ورود موفق",
                    Model = user  // ✔ مهم
                };
            }
            catch (Exception ex)
            {
                return new BaseResult
                {
                    Status = false,
                    Message = "خطا در ارتباط با سرویس احراز هویت",
                    Model = ex.ToString()
                };
            }
        }









        /// <summary>
        /// گرفتن اطلاعات لاگین کاربر
        /// </summary>
        /// <param name="Username">نام کاربری</param>
        /// <param name="Password">کلمه عبور</param>
        /// <returns></returns>
        //public BaseResult Login(string Username = null, string Password = null, int? UserId = null)
        //{
        //    Username = Username?.Trim().ToLower().ToEnglishNumber().ToPersianCharacter();
        //    string HashPassword = Password?.GetHash();

        //    if (string.IsNullOrEmpty(Username) && string.IsNullOrEmpty(Password) && UserId == null)
        //        return new BaseResult { Status = false, Message = "نام کاربری و کلمه عبور را وارد کنید" };

        //    #region گرفتن اطلاعات کاربر و اعتبار سنجی
        //    var filter = PredicateBuilder.New<User>(true);
        //    if (UserId != null)
        //        filter.And(x => x.Id == UserId);
        //    else
        //        filter.And(x => x.Username == Username && x.Password == HashPassword);

        //    var user = UOW.Users.GetOneDTO<UserSessionDTO>(UserSessionDTO.Selector, filter);

        //    if (user == null)
        //        return new BaseResult
        //        {
        //            Status = false,
        //            Message = "حساب کاربری شما یافت نشد!"
        //        };
        //    if (!user.IsEnabled)
        //        return new BaseResult
        //        {
        //            Status = false,
        //            Message = "حساب کاربری شما فعال نمی باشد! </br> جهت فعالسازی با پشتیبانی تماس بگیرید.",
        //            Model = user
        //        };
        //    #endregion


        //    #region بررسی دوره تغییر کلمه عبور توسط کاربر
        //    if (user.PasswordIsChanged)
        //    {
        //        var changePassCycle = UOW.Constants.GetChangePasswordCycle() ?? 60;
        //        if (user.ChangePasswordCycle != null)
        //        {
        //            var cycle = changePassCycle;
        //            if (user.ChangePasswordCycle != null && user.ChangePasswordCycle < changePassCycle)
        //                cycle = (int)user.ChangePasswordCycle;
        //            var LastChangePassword = UOW.UserLogs.FirstOrDefault(x => x.IsSuccess && x.MenuType == MenuType.Profile && x.ActionType == ActionType.ChangePassword && x.TargetId == user.Id);
        //            // اگر کلمه عبور را تغییر نداده بود و یا به اندازه یک سیکل از اخرین تغییر آن گذشته بود
        //            if (LastChangePassword == null || LastChangePassword.CreateDate.AddDays(cycle) < DateTime.Now)
        //                user.PasswordIsChanged = false;
        //        }
        //    }
        //    #endregion


        //    Session.SetUser(user);

        //    return new BaseResult
        //    {
        //        Status = true,
        //        Message = "کاربر گرامی " + user.FullName + "، خوش آمدید.",
        //        Model = user
        //    };
        //}


        /// <summary>
        /// گرفتن اطلاعات لازم برای ذخیره کاربر در سشن
        /// </summary>
        /// <param name="id">شناسه کاربر</param>
        /// <returns></returns>
        public UserSessionDTO GetSessionDTO(int id)
        {
            return UOW.Users.GetOneDTO<UserSessionDTO>(UserSessionDTO.Selector, x => x.Id == id);
        }




        /// <summary>
        /// لاگ اوت کاربر
        /// </summary>
        public void Logout()
        {
            Session.RemoveUser();
        }



        /// <summary>
        ///  داده خامی که از منوهای کاربر دریافت می شود،
        ///  به صورت ساختار درختی واقعی در می آید
        /// </summary>
        /// <param name="data">داده خام خارج از ساختار درختی</param>
        /// <param name="parentId">آیدی والد</param>
        /// <returns></returns>
        public List<MenuSessionDTO> ReshapeMenuData(IEnumerable<MenuSessionDTO> data, long? parentId = null)
        {
            if (data == null || data.Count() == 0)
                return null;

            var model = data.Where(x => x.ParentId == parentId && x.ShowInMenu).ToList();

            if (model == null || model.Count() == 0)
                return null;

            foreach (var item in model)
                item.SubMenus = ReshapeMenuData(data, item.Id);

            return model;
        }



    }

}
