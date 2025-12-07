using AntiXssMiddleware.Middleware;
using BLL;
using BLL.Interface;
using BLL.ManageToken;
using Filters;
using Infrastructure.Data;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Services.RedisService;
using StackExchange.Redis.Extensions.Core.Configuration;
using StackExchange.Redis.Extensions.Newtonsoft;
using StackExchange.Redis.Extensions.Utf8Json;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSignalR();

// Add services to the container.
var services = builder.Services;
services.AddControllersWithViews()
                .AddRazorRuntimeCompilation()
                .AddNewtonsoftJson(options =>
                    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
                );
services.AddRazorPages();



#region DB Context
// Main DB Connection String
var connectionString = builder.Configuration.GetConnectionString("ApplicationContext"); // خوندن از appsetting
//var connectionString = builder.Configuration["ConnectionStrings:ApplicationContext"]; // خوندن از user secret
services.AddDbContext<ApplicationContext>(options => options.UseSqlServer(connectionString)/*, ServiceLifetime.Transient*/);

// Log DB Connection String
var logConnectionString = builder.Configuration.GetConnectionString("LogContext"); // خوندن از appsetting
//var logConnectionString = builder.Configuration["ConnectionStrings:LogContext"]; // خوندن از user secret
services.AddDbContext<LogContext>(options => options.UseSqlServer(logConnectionString));
#endregion



#region redis
//var multiplexer = ConnectionMultiplexer.Connect("localhost:6379");
//services.AddSingleton<IConnectionMultiplexer>(multiplexer);

var RedisConfigurations = new List<RedisConfiguration>();
var redisConfig = builder.Configuration.GetSection("Redis").Get<RedisConfiguration>();
RedisConfigurations.Add(redisConfig);

services.AddStackExchangeRedisExtensions<Utf8JsonSerializer>((options) =>
{
    return RedisConfigurations;
}).AddStackExchangeRedisExtensions<NewtonsoftSerializer>((options) =>
{
    return RedisConfigurations;
});

services.AddSingleton<IRedisManager, RedisManager>();

#region ثبت لاگ در بکگراند
services.AddHostedService<LogBackgroundWorker>();
#endregion
#endregion



#region گرفتن httpcontext در کلاس لایبرری ها
services.AddHttpContextAccessor();
#endregion




#region اضافه کردن سرویس کوکی
//services.Configure<CookiePolicyOptions>(options =>
//{
//    options.CheckConsentNeeded = context => true;
//    options.MinimumSameSitePolicy = SameSiteMode.Unspecified;
//});
#endregion


#region اضافه کردن سرویس سشن
services.AddDistributedMemoryCache();

services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(builder.Configuration.GetValue<double?>("Setting:SessionTimeout") ?? 60);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.Name = "_session.cookie";
});
#endregion


#region اضافه کردن سرویس کش
//services.AddMemoryCache();

//services.AddResponseCaching(options =>
//{
//    options.UseCaseSensitivePaths = false;
//});
#endregion



#region فیلتر برای اکشن ها
// فیلتر بررسی لاگین ادمین در پنل ادمین
services.AddScoped<UserAuthorize>(_ => new UserAuthorize());
#endregion



#region نام کوکی و هدر AntiForgeryKey برای جلوگیری از حملات csrf
services.AddAntiforgery(options =>
{
    options.Cookie.Name = "_CSRF.cookie";
    options.HeaderName = "_CSRF_header";
});
#endregion


#region استیمول سافت
services.AddMvc(options => options.EnableEndpointRouting = false);
services.Configure<IISServerOptions>(options => options.AllowSynchronousIO = true);
services.Configure<KestrelServerOptions>(options =>
{
    options.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(10);
    options.Limits.RequestHeadersTimeout = TimeSpan.FromMinutes(10);
    options.AllowSynchronousIO = true;
});
#endregion

#region Dipendency Injection


// فکتوری برای همه کانتکست ها
services.AddScoped<DbContexts>();

#region DataTable
services.AddScoped<IDataTableManager, DataTableManager>();
#endregion


#region AuthSystem
services.AddScoped<IAuthManager, AuthManager>();
services.AddScoped<IMenuManager, MenuManager>();
services.AddScoped<IRoleManager, RoleManager>();
services.AddScoped<IRoleMenuManager, RoleMenuManager>();
services.AddScoped<IUserManager, UserManager>();
#endregion

#region LogSystem
services.AddScoped<IUserLogManager, UserLogManager>();
#endregion

#region Projects
services.AddScoped<IUnitsManager, UnitsManager>();


#endregion

#region Shared
services.AddScoped<IConstantManager, ConstantManager>();
#endregion
services.AddSingleton<ApiTokenCacheClient>();
services.AddHttpClient();
services.AddDistributedMemoryCache();

#endregion




var app = builder.Build();
app.UseHsts();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthorization();

#region امنیت
// برای جلوگیری از حملات xss
app.UseAntiXssMiddleware();

#region تنظیم هدر برای جلوگیری از حملات
app.Use(async (context, next) =>
{
    // برای جلوگیری از iframe شدن صفحات سایت و براي مقابله در برابر حملات ClickJacking
    context.Response.Headers.Add("X-Frame-Options", "DENY");

    // جلوگیری از حملات xss
    context.Response.Headers.Add("X-Xss-Protection", "1; mode=block");

    // جلوگیری از MIME-Sniffing و تغییر پسوند فایل ها
    context.Response.Headers.Add("X-Content-Type-Options", "nosniff");

    context.Response.Headers.Remove("Server");
    await next();
});
#endregion 
#endregion



#region Exception Handler
//app.UseExceptionHandler("/error");
app.UseExceptionHandler(
          new ExceptionHandlerOptions()
          {
              AllowStatusCode404Response = true,
              ExceptionHandlingPath = "/error"
          }
      );
#endregion


app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Authentication}/{action=Index}/{id?}");
#region استیمول سافت
app.UseMvc(routes =>
{
    routes.MapRoute(
        name: "default",
    template: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");
});
#endregion

app.Run();
