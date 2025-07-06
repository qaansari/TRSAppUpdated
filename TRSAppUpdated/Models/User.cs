using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TRSAppUpdated.Models
{
    public class User
    {
        [Key]
        public int ID { get; set; }
        [Column(TypeName = "varchar(20)")]
        public string FirstName { get; set; }
        [Column(TypeName = "varchar(20)")]
        public string LastName { get; set; }
        [Column(TypeName = "varchar(41)")]
        public string FullName
        {
            get
            {
                return FirstName + " " + LastName;
            }
        }
        [Column(TypeName = "varchar(20)")]
        public string ContactNumber { get; set; }
        [Column(TypeName = "varchar(320)")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Column(TypeName = "varchar(max)")]
        public string Password { get; set; }
        [ForeignKey("Role")]
        public int RoleID { get; set; }
        [Column(TypeName = "varchar(300)")]
        public string? ImagePath { get; set; }
        [Column(TypeName = "varchar(68)")]
        public string? UserToken { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsVerified { get; set; }
        [ForeignKey("UserCreatedByUser")]
        public int? CreatedByUserID { get; set; }
        [ForeignKey("UserUpdatedByUser")]
        public int? UpdatedByUserID { get; set; }
        [ForeignKey("UserDeletedByUser")]
        public int? DeletedByUserID { get; set; }
        public DateTime? CreatedDateTime { get; set; }
        public DateTime? UpdatedDateTime { get; set; }
        public DateTime? DeletedDateTime { get; set; }
        public User? UserCreatedByUser { get; set; }
        public User? UserUpdatedByUser { get; set; }
        public User? UserDeletedByUser { get; set; }
        public Role Role { get; set; }
        //Inverse props
        [InverseProperty("UserCreatedByUser")]
        public ICollection<User> UsersCreated { get; set; }
        [InverseProperty("UserUpdatedByUser")]
        public ICollection<User> UsersUpdated { get; set; }
        [InverseProperty("UserDeletedByUser")]
        public ICollection<User> UsersDeleted { get; set; }
        [InverseProperty("CreatedByUser")]
        public ICollection<Auditorium> AuditoriumsCreated { get; set; }
        [InverseProperty("UpdatedByUser")]
        public ICollection<Auditorium> AuditoriumsUpdated { get; set; }
        [InverseProperty("DeletedByUser")]
        public ICollection<Auditorium> AuditoriumsDeleted { get; set; }
        [InverseProperty("CreatedByUser")]
        public ICollection<Category> CategoriesCreated { get; set; }
        [InverseProperty("UpdatedByUser")]
        public ICollection<Category> CategoriesUpdated { get; set; }
        [InverseProperty("DeletedByUser")]
        public ICollection<Category> CategoriesDeleted { get; set; }
        [InverseProperty("CreatedByUser")]
        public ICollection<City> CitiesCreated { get; set; }
        [InverseProperty("UpdatedByUser")]
        public ICollection<City> CitiesUpdated { get; set; }
        [InverseProperty("DeletedByUser")]
        public ICollection<City> CitiesDeleted { get; set; }
        [InverseProperty("CreatedByUser")]
        public ICollection<Column> ColumnsCreated { get; set; }
        [InverseProperty("CreatedByUser")]
        public ICollection<Country> CountriesCreated { get; set; }
        [InverseProperty("UpdatedByUser")]
        public ICollection<Country> CountriesUpdated { get; set; }
        [InverseProperty("DeletedByUser")]
        public ICollection<Country> CountriesDeleted { get; set; }
        [InverseProperty("CreatedByUser")]
        public ICollection<Event> EventsCreated { get; set; }
        [InverseProperty("UpdatedByUser")]
        public ICollection<Event> EventsUpdated { get; set; }
        [InverseProperty("DeletedByUser")]
        public ICollection<Event> EventsDeleted { get; set; }
        [InverseProperty("CreatedByUser")]
        public ICollection<EventCategory> EventCategoriesCreated { get; set; }
        [InverseProperty("CreatedByUser")]
        public ICollection<EventStatus> EventStatusesCreated { get; set; }
        [InverseProperty("UpdatedByUser")]
        public ICollection<EventStatus> EventStatusesUpdated { get; set; }
        [InverseProperty("DeletedByUser")]
        public ICollection<EventStatus> EventStatusesDeleted { get; set; }
        [InverseProperty("CreatedByUser")]
        public ICollection<Expense> ExpensesCreated { get; set; }
        [InverseProperty("UpdatedByUser")]
        public ICollection<Expense> ExpensesUpdated { get; set; }
        [InverseProperty("DeletedByUser")]
        public ICollection<Expense> ExpensesDeleted { get; set; }
        [InverseProperty("CreatedByUser")]
        public ICollection<Note> NotesCreated { get; set; }
        [InverseProperty("UpdatedByUser")]
        public ICollection<Note> NotesUpdated { get; set; }
        [InverseProperty("DeletedByUser")]
        public ICollection<Note> NotesDeleted { get; set; }
        [InverseProperty("CreatedByUser")]
        public ICollection<NoteType> NoteTypesCreated { get; set; }
        [InverseProperty("UpdatedByUser")]
        public ICollection<NoteType> NoteTypesUpdated { get; set; }
        [InverseProperty("DeletedByUser")]
        public ICollection<NoteType> NoteTypesDeleted { get; set; }
        [InverseProperty("CreatedByUser")]
        public ICollection<PaymentMode> PaymentModesCreated { get; set; }
        [InverseProperty("UpdatedByUser")]
        public ICollection<PaymentMode> PaymentModesUpdated { get; set; }
        [InverseProperty("DeletedByUser")]
        public ICollection<PaymentMode> PaymentModesDeleted { get; set; }
        [InverseProperty("CreatedByUser")]
        public ICollection<Reservation> ReservationsCreated { get; set; }
        [InverseProperty("UpdatedByUser")]
        public ICollection<Reservation> ReservationsUpdated { get; set; }
        [InverseProperty("DeletedByUser")]
        public ICollection<Reservation> ReservationsDeleted { get; set; }
        [InverseProperty("CreatedByUser")]
        public ICollection<ReservationImage> ReservationImagesCreated { get; set; }
        [InverseProperty("CreatedByUser")]
        public ICollection<ReservationTicket> ReservationTicketsCreated { get; set; }
        [InverseProperty("CreatedByUser")]
        public ICollection<Row> RowsCreated { get; set; }
        [InverseProperty("CreatedByUser")]
        public ICollection<Seat> SeatsCreated { get; set; }
        [InverseProperty("CreatedByUser")]
        public ICollection<Show> ShowsCreated { get; set; }
        [InverseProperty("UpdatedByUser")]
        public ICollection<Show> ShowsUpdated { get; set; }
        [InverseProperty("DeletedByUser")]
        public ICollection<Show> ShowsDeleted { get; set; }
        [InverseProperty("CreatedByUser")]
        public ICollection<ShowExpense> ShowExpensesCreated { get; set; }
        [InverseProperty("UpdatedByUser")]
        public ICollection<ShowExpense> ShowExpensesUpdated { get; set; }
        [InverseProperty("DeletedByUser")]
        public ICollection<ShowExpense> ShowExpensesDeleted { get; set; }
        [InverseProperty("CreatedByUser")]
        public ICollection<Ticket> TicketsCreated { get; set; }
        [InverseProperty("UpdatedByUser")]
        public ICollection<Ticket> TicketsUpdated { get; set; }
        [InverseProperty("DeletedByUser")]
        public ICollection<Ticket> TicketsDeleted { get; set; }
        [InverseProperty("CreatedByUser")]
        public ICollection<Venue> VenuesCreated { get; set; }
        [InverseProperty("UpdatedByUser")]
        public ICollection<Venue> VenuesUpdated { get; set; }
        [InverseProperty("DeletedByUser")]
        public ICollection<Venue> VenuesDeleted { get; set; }
    }
}
