using Microsoft.EntityFrameworkCore;
using TRSAppUpdated.Helpers;

namespace TRSAppUpdated.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<Auditorium> Auditoriums { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Column> Columns { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<EventCategory> EventCategories { get; set; }
        public DbSet<EventStatus> EventStatuses { get; set; }
        public DbSet<Expense> Expenses { get; set; }
        public DbSet<Note> Notes { get; set; }
        public DbSet<NoteType> NoteTypes { get; set; }
        public DbSet<PaymentMode> PaymentModes { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<ReservationImage> ReservationImages { get; set; }
        public DbSet<ReservationStatus> ReservationStatuses { get; set; }
        public DbSet<ReservationTicket> ReservationTickets { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Row> Rows { get; set; }
        public DbSet<Seat> Seats { get; set; }
        public DbSet<Show> Shows { get; set; }
        public DbSet<ShowExpense> ShowExpenses { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Venue> Venues { get; set; }
        public DbSet<User> Users { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Role>().HasData(
                new Role()
                {
                    ID = 1,
                    Title = "Super Admin",
                    IsDeleted = false,
                },
                 new Role()
                 {
                     ID = 2,
                     Title = "Admin",
                     IsDeleted = false,
                 },
                  new Role()
                  {
                      ID = 3,
                      Title = "Moderator",
                      IsDeleted = false,
                  },
                   new Role()
                   {
                       ID = 4,
                       Title = "User",
                       IsDeleted = false,
                   }
            );
            modelBuilder.Entity<User>().HasData(
               new User()
               {
                   ID = 1,
                   RoleID = 1,
                   FirstName = "Super",
                   LastName = "Admin",
                   ContactNumber = "+923312541767",
                   Email = "sa@eventland.pk",
                   Password = "sa@eventland!23..#".HashWithSalt(),
                   IsActive = true,
                   IsVerified = true,
                   IsDeleted = false,
                   CreatedDateTime = DateTime.UtcNow.AddHours(5),
               });
            modelBuilder.Entity<EventStatus>().HasData(
                new EventStatus()
                {
                    ID = 1,
                    Title = "Active",
                    CreatedByUserID = 1,
                    CreatedDateTime = DateTime.UtcNow.AddHours(5),
                    IsDeleted = false,
                },
                 new EventStatus()
                 {
                     ID = 2,
                     Title = "Ended",
                     CreatedByUserID = 1,
                     CreatedDateTime = DateTime.UtcNow.AddHours(5),
                     IsDeleted = false,
                 },
                  new EventStatus()
                  {
                      ID = 3,
                      Title = "Postponed",
                      CreatedByUserID = 1,
                      CreatedDateTime = DateTime.UtcNow.AddHours(5),
                      IsDeleted = false,
                  });
            modelBuilder.Entity<Country>().HasData(
                new Country()
                {
                    ID = 1,
                    Title = "Pakistan",
                    DialingCode = "+92",
                    Flag = "🇵🇰",
                    CreatedByUserID = 1,
                    CreatedDateTime = DateTime.UtcNow.AddHours(5),
                    IsDeleted = false,
                });
            modelBuilder.Entity<City>().HasData(
                new City()
                {
                    ID = 1,
                    Title = "Karachi",
                    CountryID = 1,
                    CreatedByUserID = 1,
                    CreatedDateTime = DateTime.UtcNow.AddHours(5),
                    IsDeleted = false,
                },
                new City()
                {
                    ID = 2,
                    Title = "Lahore",
                    CountryID = 1,
                    CreatedByUserID = 1,
                    CreatedDateTime = DateTime.UtcNow.AddHours(5),
                    IsDeleted = false,
                },
                new City()
                {
                    ID = 3,
                    Title = "Islamabad",
                    CountryID = 1,
                    CreatedByUserID = 1,
                    CreatedDateTime = DateTime.UtcNow.AddHours(5),
                    IsDeleted = false,
                });
            modelBuilder.Entity<PaymentMode>().HasData(
                new PaymentMode()
                {
                    ID = 1,
                    Title = "Cash",
                    CreatedByUserID = 1,
                    CreatedDateTime = DateTime.UtcNow.AddHours(5),
                    IsDeleted = false,
                },
                new PaymentMode()
                {
                    ID = 2,
                    Title = "Easypaisa/Jazzcash",
                    CreatedByUserID = 1,
                    CreatedDateTime = DateTime.UtcNow.AddHours(5),
                    IsDeleted = false,
                },
                new PaymentMode()
                {
                    ID = 3,
                    Title = "Bank Transfer",
                    CreatedByUserID = 1,
                    CreatedDateTime = DateTime.UtcNow.AddHours(5),
                    IsDeleted = false,
                });
            modelBuilder.Entity<ReservationStatus>().HasData(
                new ReservationStatus()
                {
                    ID = 1,
                    Title = "Arrival Pending",
                    CreatedByUserID = 1,
                    CreatedDateTime = DateTime.UtcNow.AddHours(5),
                    IsDeleted = false,
                },
                new ReservationStatus()
                {
                    ID = 2,
                    Title = "Cancelled",
                    CreatedByUserID = 1,
                    CreatedDateTime = DateTime.UtcNow.AddHours(5),
                    IsDeleted = false,
                },
                new ReservationStatus()
                {
                    ID = 3,
                    Title = "Closed",
                    CreatedByUserID = 1,
                    CreatedDateTime = DateTime.UtcNow.AddHours(5),
                    IsDeleted = false,
                },
                new ReservationStatus()
                {
                    ID = 4,
                    Title = "Refunded",
                    CreatedByUserID = 1,
                    CreatedDateTime = DateTime.UtcNow.AddHours(5),
                    IsDeleted = false,
                });
        }

    }
}
