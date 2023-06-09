
using Microsoft.AspNetCore.Identity;
using NET6.Data.Context;
using NET6.Data.Extensions;
using NET6.Entity.Entities;
using NET6.Service.Describers;
using NET6.Service.Extensions;
using NET6.Web.Filters.ArticleVisitors;
using NToastNotify;
using NLog;

using NLog.Web;
using Microsoft.AspNetCore.Rewrite;

internal class Program
{

    private static void Main(string[] args)
    {
        var logger = NLog.LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
        logger.Debug("init main");
        try
        {
            var builder = WebApplication.CreateBuilder(args);

            object value = builder.Services.LoadDataLayerExtension(builder.Configuration);
            builder.Services.LoadServiceLayerExtension();
            builder.Services.AddSession();
            // Add services to the container.
            builder.Services.AddControllersWithViews(opt =>
            {

                opt.Filters.Add<ArticleVisitorFilter>();
                opt.Filters.Add<PortfolioVisitorFilter>();

            })
                .AddNToastNotifyToastr(new ToastrOptions()
                {
                    PositionClass = ToastPositions.TopRight,
                    TimeOut = 3000,
                })
                .AddRazorRuntimeCompilation();


            builder.Services.AddIdentity<AppUser, AppRole>(opt =>
            {
                opt.Password.RequireNonAlphanumeric = false;
                opt.Password.RequireLowercase = false;
                opt.Password.RequireUppercase = false;
            })
                .AddRoleManager<RoleManager<AppRole>>()
                .AddErrorDescriber<CustomIdentityErrorDescriber>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            builder.Services.ConfigureApplicationCookie(config =>
            {
                config.LoginPath = new PathString("/Admin/Auth/Login");
                config.LogoutPath = new PathString("/Admin/Auth/Logout");
                config.Cookie = new CookieBuilder
                {
                    Name = "NET6",
                    HttpOnly = true,
                    SameSite = SameSiteMode.Strict,
                    SecurePolicy = CookieSecurePolicy.SameAsRequest //Always 
                };
                config.SlidingExpiration = true;
                config.ExpireTimeSpan = TimeSpan.FromDays(7);
                config.AccessDeniedPath = new PathString("/Admin/Auth/AccessDenied");
            });

            // NLog: Setup NLog for Dependency injection
            builder.Logging.ClearProviders();
            builder.Host.UseNLog();


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseNToastNotify();
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSession();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapAreaControllerRoute(
                    name: "Admin",
                    areaName: "Admin",
                    pattern: "Admin/{controller=Home}/{action=Index}/{id?}");


                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{Controller=Home}/{Action=Index}/{id:Guid}");

                endpoints.MapDefaultControllerRoute();

            });

            app.Run();
        }
        catch (Exception exception)
        {
            // NLog: catch setup errors
            logger.Error(exception, "Stopped program because of exception");
            throw;
        }
        finally
        {
            // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
            NLog.LogManager.Shutdown();
        }
    }
}