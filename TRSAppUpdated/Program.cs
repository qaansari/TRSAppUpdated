using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
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
            builder.Services.AddSession();
            builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
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
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
            {

            });
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("SuperAdmin", policy=>policy.RequireRole("Super Admin"));
                options.AddPolicy("Admin", policy=>policy.RequireRole("Admin"));
                options.AddPolicy("Moderator", policy=>policy.RequireRole("Moderator"));
                options.AddPolicy("User", policy=>policy.RequireRole("User"));
            });
            //Json Cycles Ignore
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                Formatting = Newtonsoft.Json.Formatting.Indented,
                ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore,
            };
            //Syncfusion
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1JEaF5cXmRCf1FpRmJGdld5fUVHYVZUTXxaS00DNHVRdkdmWXhedHZWQmlZWE1yXkpWYEk=");
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
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            app.Run();
        }
    }
}
