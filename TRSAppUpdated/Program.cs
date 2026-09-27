using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Threading.RateLimiting;
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

            // Configure AppHelper static settings from configuration
            AppHelper.ConfigureSettings(builder.Configuration);

            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<AppDbContext>(options =>
            {
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString), mySqlOptions =>
                {
                    mySqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(10),
                        errorNumbersToAdd: null);
                });
                options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            });

            builder.Services.AddMemoryCache();
            builder.Services.AddSingleton<ICacheService, CacheService>();
            builder.Services.AddControllersWithViews(options =>
            {
                options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
            });

            builder.Services.AddRateLimiter(options =>
            {
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
                options.AddFixedWindowLimiter("auth-limit", opt =>
                {
                    opt.PermitLimit = 20;
                    opt.Window = TimeSpan.FromMinutes(1);
                    opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                    opt.QueueLimit = 0;
                });
            });

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
         options.Cookie.Name = "AppAuth";
         options.Cookie.HttpOnly = true;
         options.Cookie.IsEssential = true;
         options.Cookie.SecurePolicy = builder.Environment.IsDevelopment()
             ? CookieSecurePolicy.SameAsRequest
             : CookieSecurePolicy.Always; // Use Always in production

         options.LoginPath = "/Home/Index";
         options.LogoutPath = "/Home/Logout";
         options.AccessDeniedPath = "/Home/Index";

         // Use absolute times for server stability
         options.ExpireTimeSpan = TimeSpan.FromDays(30); // Max you want
         options.SlidingExpiration = true;

         // IMPORTANT for HosterPK: Set explicit cookie expiration
         options.Events = new CookieAuthenticationEvents
         {
             OnSigningIn = async context =>
             {
                 // Set cookie expiry to match your logic
                 if (context.Properties.IsPersistent)
                 {
                     context.Properties.ExpiresUtc = DateTimeOffset.UtcNow.AddDays(1);
                 }
                 else
                 {
                     context.Properties.ExpiresUtc = DateTimeOffset.UtcNow.AddHours(1);
                 }
                 await Task.CompletedTask;
             }
         };
     });
            //Session Configuration
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromHours(24);  // Match cookie timeout
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
                options.Cookie.Name = "AppSession";
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
            var nativeLibPath = Path.Combine(builder.Environment.WebRootPath, "assets", "native", "libwkhtmltox.dll");
            if (File.Exists(nativeLibPath))
            {
                var context = new CustomAssemblyLoadContext();
                context.LoadUnmanagedLibrary(nativeLibPath);
            }
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

            // Security Headers Middleware
            app.Use(async (context, next) =>
            {
                context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
                context.Response.Headers.Append("X-Frame-Options", "SAMEORIGIN");
                context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");
                context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");
                await next();
            });

            // Request Localization to ensure standard datetime format parsing (en-US / ISO)
            var defaultCulture = new System.Globalization.CultureInfo("en-US");
            var localizationOptions = new RequestLocalizationOptions
            {
                DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture(defaultCulture),
                SupportedCultures = new List<System.Globalization.CultureInfo> { defaultCulture, new System.Globalization.CultureInfo("en-GB") },
                SupportedUICultures = new List<System.Globalization.CultureInfo> { defaultCulture, new System.Globalization.CultureInfo("en-GB") }
            };
            app.UseRequestLocalization(localizationOptions);

            app.UseRouting();

            app.UseRateLimiter();
            app.UseSession();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            app.Run();
        }
    }
}
