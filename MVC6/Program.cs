using Microsoft.AspNetCore.Authentication.Cookies;
using MVC6.Controllers.Interfaces;
using MVC6.Controllers.Services;
using MVC6.Models;
using MVC6.Models.Providers;
using MVC6.Utilities;
using NLog;
using NLog.Web;

var logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IUser, UserService>();
builder.Services.AddScoped<ICart, CartService>();
builder.Services.AddScoped<IBoard, BoardService>();
builder.Services.AddScoped<IGrant, GrantService>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IDBConnection, PostgresProvider>();

//NLog
builder.Logging.ClearProviders();
builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
builder.Host.UseNLog();

//Add Accessor
builder.Services.AddHttpContextAccessor();

Common.SetDataProtection(builder.Services, @"./Utilities/DataProtector", "MVC", Enums.CryptoType.Managed);

builder.Services.AddAuthentication(defaultScheme: CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.AccessDeniedPath = "/Membership/Forbidden";
        options.LoginPath = "/Membership/Login";
    });

builder.Services.AddAuthorization();

//Add Memory and Session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options => {
    options.Cookie.Name = "WebSession";
    options.IdleTimeout = TimeSpan.FromMinutes(30);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

//Add
app.UseAuthentication();

app.UseAuthorization();

//Add Session
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();


//NLog
LogManager.Shutdown();

