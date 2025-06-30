using Microsoft.EntityFrameworkCore;

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
                ID=1,
                Title="Super Admin",
                IsDeleted=false,
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
                   FirstName="Super",
                   LastName="Admin",
                   IsDeleted = false,
               });
        }

    }
}
