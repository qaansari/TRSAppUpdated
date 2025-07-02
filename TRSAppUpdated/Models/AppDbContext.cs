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
        public DbSet<Column> Columns { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<EventCategory> EventCategories { get; set; }
        public DbSet<Expense> Expenses { get; set; }
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
               },
                new User()
                {
                    ID = 2,
                    RoleID = 2,
                    FirstName = "Qamar",
                    LastName = "Ansari",
                    ContactNumber = "+923312541767",
                    Email = "qa.ansari80@eventland.pk",
                    Password = "qamaransari04012002".HashWithSalt(),
                    IsActive = true,
                    IsVerified = true,
                    IsDeleted = false,
                    CreatedByUserID = 1,
                    CreatedDateTime = DateTime.UtcNow.AddHours(5),
                },
               new User()
               {
                   ID = 3,
                   RoleID = 2,
                   FirstName = "Tayyab",
                   LastName = "Ahmed",
                   ContactNumber = "+923162361582",
                   Email = "stayyabahmed85@eventland.pk",
                   Password = "tayyabahmed07111999".HashWithSalt(),
                   IsActive = true,
                   IsVerified = true,
                   IsDeleted = false,
                   CreatedByUserID = 1,
                   CreatedDateTime = DateTime.UtcNow.AddHours(5),
               },
               new User()
               {
                   ID = 4,
                   RoleID = 2,
                   FirstName = "Anas",
                   LastName = "Ali",
                   ContactNumber = "+923091265921",
                   Email = "anasali4772@eventland.pk",
                   Password = "anasali02012001".HashWithSalt(),
                   IsActive = true,
                   IsVerified = true,
                   IsDeleted = false,
                   CreatedByUserID = 1,
                   CreatedDateTime = DateTime.UtcNow.AddHours(5),
               });
            modelBuilder.Entity<PaymentMode>().HasData(
               new PaymentMode()
               {
                   ID = 1,
                   Title = "Cash",
                   CreatedByUserID = 1,
                   CreatedDateTime = DateTime.UtcNow.AddHours(5),
               },
                new PaymentMode()
                {
                    ID = 2,
                    Title = "Easypaisa",
                    CreatedByUserID = 1,
                    CreatedDateTime = DateTime.UtcNow.AddHours(5),
                },
                 new PaymentMode()
                 {
                     ID = 3,
                     Title = "Jazzcash",
                     CreatedByUserID = 1,
                     CreatedDateTime = DateTime.UtcNow.AddHours(5),
                 },
                  new PaymentMode()
                  {
                      ID = 4,
                      Title = "Bank Transfer",
                      CreatedByUserID = 1,
                      CreatedDateTime = DateTime.UtcNow.AddHours(5),
                  });
            modelBuilder.Entity<ReservationStatus>().HasData(
              new ReservationStatus()
              {
                  ID = 1,
                  Title = "Arrival Pending",
                  CreatedByUserID = 1,
                  CreatedDateTime = DateTime.UtcNow.AddHours(5),
              },
               new ReservationStatus()
               {
                   ID = 2,
                   Title = "Not Attended",
                   CreatedByUserID = 1,
                   CreatedDateTime = DateTime.UtcNow.AddHours(5),
               },
                new ReservationStatus()
                {
                    ID = 3,
                    Title = "Closed",
                    CreatedByUserID = 1,
                    CreatedDateTime = DateTime.UtcNow.AddHours(5),
                },
                new ReservationStatus()
                {
                    ID = 4,
                    Title = "Refunded",
                    CreatedByUserID = 1,
                    CreatedDateTime = DateTime.UtcNow.AddHours(5),
                });
        }

    }
}
