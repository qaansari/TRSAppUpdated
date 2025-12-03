using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using TRSAppUpdated.Helpers;
using TRSAppUpdated.Interfaces;
using TRSAppUpdated.Models;
using TRSAppUpdated.Services;

namespace TRSAppUpdated
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            // Add services to the container.
            builder.Services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
                options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            });
            builder.Services.AddControllersWithViews();
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            builder.Services.AddScoped<IExpenseService, ExpenseService>();
            builder.Services.AddScoped<IRoleService, RoleService>();
            builder.Services.AddScoped<IVenueService, VenueService>();
            builder.Services.AddScoped<IAuditoriumService, AuditoriumService>();
            builder.Services.AddScoped<ICategoryService, CategoryService>();
            builder.Services.AddScoped<ICountryService, CountryService>();
            builder.Services.AddScoped<ICityService, CityService>();
            builder.Services.AddScoped<IEventStatusService, EventStatusService>();
            builder.Services.AddScoped<IEventService, EventService>();
            builder.Services.AddScoped<INoteTypeService, NoteTypeService>();
            builder.Services.AddScoped<INoteService, NoteService>();
            builder.Services.AddScoped<IPaymentModeService, PaymentModeService>();
            builder.Services.AddScoped<IReservationService, ReservationService>();
            builder.Services.AddScoped<IReservationStatusService, ReservationStatusService>();
            builder.Services.AddScoped<IShowService, ShowService>();
            builder.Services.AddScoped<ITicketService, TicketService>();
            builder.Services.AddScoped<IUserService, UserService>();
            //Authentication Configuration
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "TRSAppUpdated.AuthCookie";
        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest; // Adjust based on your SSL
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.LoginPath = "/Home/Index";
        options.LogoutPath = "/Home/Logout";
        options.AccessDeniedPath = "/Home/Index";
        // IMPORTANT: Set these values to match your login logic
        options.ExpireTimeSpan = TimeSpan.FromHours(24);  // Default to 24 hours
        options.SlidingExpiration = true; // This resets expiration on activity
    });
            //Session Configuration
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromHours(24);  // Match cookie timeout
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
                options.Cookie.Name = "TRSAppUpdated.Session";
                options.Cookie.SameSite = SameSiteMode.Lax;
                // Session cookie should not expire when browser closes
                options.Cookie.MaxAge = TimeSpan.FromHours(24);
            });
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("SuperAdmin", policy => policy.RequireRole("Super Admin"));
                options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
                options.AddPolicy("Moderator", policy => policy.RequireRole("Moderator"));
                options.AddPolicy("User", policy => policy.RequireRole("User"));
            });
            //Json Cycles Ignore
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                Formatting = Newtonsoft.Json.Formatting.Indented,
                ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore,
            };
            //Syncfusion
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1JEaF5cXmRCf1FpRmJGdld5fUVHYVZUTXxaS00DNHVRdkdmWXhedHZWQmlZWE1yXkpWYEk=");
            // Register DinkToPdf
            var context = new CustomAssemblyLoadContext();
            context.LoadUnmanagedLibrary(Path.Combine(builder.Environment.WebRootPath,"assets","native", "libwkhtmltox.dll"));
            builder.Services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));
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
            app.UseAuthentication();
            app.UseRouting();
            app.UseAuthorization();
            app.UseSession();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            app.Run();
        }
    }
}
