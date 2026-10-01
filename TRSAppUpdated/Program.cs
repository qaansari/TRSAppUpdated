using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using QuestPDF.Infrastructure;
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

            // Configure dynamic port for Railway container environment
            var port = Environment.GetEnvironmentVariable("PORT");
            if (!string.IsNullOrEmpty(port))
            {
                builder.WebHost.UseUrls($"http://0.0.0.0:{port}");
            }

            // Configure AppHelper static settings from configuration
            AppHelper.ConfigureSettings(builder.Configuration);

            // Resolve connection string (supports standard configuration, Railway MYSQL_URL / DATABASE_URL)
            var connectionString = ResolveConnectionString(builder.Configuration);

            // Add services to the container.
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

            // Rate Limiter configuration for Security & Protection
            builder.Services.AddRateLimiter(options =>
            {
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

                // Auth limiter
                options.AddFixedWindowLimiter("auth-limit", opt =>
                {
                    opt.PermitLimit = 20;
                    opt.Window = TimeSpan.FromMinutes(1);
                    opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                    opt.QueueLimit = 0;
                });

                // OCR processing limiter (protects CPU & RAM on Railway)
                options.AddFixedWindowLimiter("ocr-limit", opt =>
                {
                    opt.PermitLimit = 15;
                    opt.Window = TimeSpan.FromMinutes(1);
                    opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                    opt.QueueLimit = 2;
                });

                // PDF generation limiter
                options.AddFixedWindowLimiter("pdf-limit", opt =>
                {
                    opt.PermitLimit = 30;
                    opt.Window = TimeSpan.FromMinutes(1);
                    opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                    opt.QueueLimit = 5;
                });
            });

            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            // Scoped business services
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

            // Cross-platform PDF and OCR Services (Scalable & Railway-friendly)
            QuestPDF.Settings.License = LicenseType.Community;
            QuestPDF.Settings.UseSystemFonts = true;
            QuestPDF.Settings.ThrowOnMissingFontFamilies = false;
            builder.Services.AddSingleton<IPdfService, QuestPdfTicketService>();
            builder.Services.AddSingleton<IOcrService, TesseractOcrService>();

            // Authentication Configuration
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.Cookie.Name = "AppAuth";
                    options.Cookie.HttpOnly = true;
                    options.Cookie.IsEssential = true;
                    options.Cookie.SecurePolicy = builder.Environment.IsDevelopment()
                        ? CookieSecurePolicy.SameAsRequest
                        : CookieSecurePolicy.Always;

                    options.LoginPath = "/Home/Index";
                    options.LogoutPath = "/Home/Logout";
                    options.AccessDeniedPath = "/Home/Index";

                    options.ExpireTimeSpan = TimeSpan.FromDays(30);
                    options.SlidingExpiration = true;

                    options.Events = new CookieAuthenticationEvents
                    {
                        OnSigningIn = async context =>
                        {
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

            // Session Configuration
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromHours(24);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
                options.Cookie.Name = "AppSession";
                options.Cookie.SameSite = SameSiteMode.Lax;
                options.Cookie.MaxAge = TimeSpan.FromHours(24);
            });

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("SuperAdmin", policy => policy.RequireRole("Super Admin"));
                options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
                options.AddPolicy("Moderator", policy => policy.RequireRole("Moderator"));
                options.AddPolicy("User", policy => policy.RequireRole("User"));
            });

            // JSON Serialization settings
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            };

            // Syncfusion
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1JEaF5cXmRCf1FpRmJGdld5fUVHYVZUTXxaS00DNHVRdkdmWXhedHZWQmlZWE1yXkpWYEk=");

            var app = builder.Build();

            // Handle forwarded headers from Railway reverse proxy
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
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

            // Request Localization
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

        private static string ResolveConnectionString(IConfiguration configuration)
        {
            var configuredConnStr = configuration.GetConnectionString("DefaultConnection");

            var envDbUrl = Environment.GetEnvironmentVariable("MYSQL_URL") ??
                           Environment.GetEnvironmentVariable("DATABASE_URL");

            if (!string.IsNullOrEmpty(envDbUrl) && envDbUrl.StartsWith("mysql://", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    var uri = new Uri(envDbUrl);
                    var userInfo = uri.UserInfo.Split(':');
                    var user = userInfo[0];
                    var password = userInfo.Length > 1 ? userInfo[1] : string.Empty;
                    var host = uri.Host;
                    var dbPort = uri.Port > 0 ? uri.Port : 3306;
                    var database = uri.AbsolutePath.TrimStart('/');

                    return $"Server={host};Port={dbPort};Database={database};User={user};Password={password};CharSet=utf8mb4;TreatTinyAsBoolean=true;";
                }
                catch
                {
                    // Fall back to configured connection string if parsing fails
                }
            }

            return configuredConnStr ?? string.Empty;
        }
    }
}
