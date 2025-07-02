using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TRSAppUpdated.Migrations
{
    /// <inheritdoc />
    public partial class DbInit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "varchar(20)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "varchar(20)", nullable: false),
                    LastName = table.Column<string>(type: "varchar(20)", nullable: false),
                    ContactNumber = table.Column<string>(type: "varchar(20)", nullable: false),
                    Email = table.Column<string>(type: "varchar(320)", nullable: false),
                    Password = table.Column<string>(type: "varchar(max)", nullable: false),
                    RoleID = table.Column<int>(type: "int", nullable: false),
                    ImagePath = table.Column<string>(type: "varchar(300)", nullable: true),
                    UserToken = table.Column<string>(type: "varchar(68)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    IsVerified = table.Column<bool>(type: "bit", nullable: false),
                    CreatedByUserID = table.Column<int>(type: "int", nullable: true),
                    UpdatedByUserID = table.Column<int>(type: "int", nullable: true),
                    DeletedByUserID = table.Column<int>(type: "int", nullable: true),
                    CreatedDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedDateTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Users_Roles_RoleID",
                        column: x => x.RoleID,
                        principalTable: "Roles",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Users_Users_CreatedByUserID",
                        column: x => x.CreatedByUserID,
                        principalTable: "Users",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_Users_Users_DeletedByUserID",
                        column: x => x.DeletedByUserID,
                        principalTable: "Users",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_Users_Users_UpdatedByUserID",
                        column: x => x.UpdatedByUserID,
                        principalTable: "Users",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "varchar(20)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedByUserID = table.Column<int>(type: "int", nullable: false),
                    UpdatedByUserID = table.Column<int>(type: "int", nullable: true),
                    DeletedByUserID = table.Column<int>(type: "int", nullable: true),
                    CreatedDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedDateTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Categories_Users_CreatedByUserID",
                        column: x => x.CreatedByUserID,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Categories_Users_DeletedByUserID",
                        column: x => x.DeletedByUserID,
                        principalTable: "Users",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_Categories_Users_UpdatedByUserID",
                        column: x => x.UpdatedByUserID,
                        principalTable: "Users",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "EventCategories",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryID = table.Column<int>(type: "int", nullable: false),
                    EventID = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedByUserID = table.Column<int>(type: "int", nullable: false),
                    UpdatedByUserID = table.Column<int>(type: "int", nullable: true),
                    DeletedByUserID = table.Column<int>(type: "int", nullable: true),
                    CreatedDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedDateTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventCategories", x => x.ID);
                    table.ForeignKey(
                        name: "FK_EventCategories_Users_CreatedByUserID",
                        column: x => x.CreatedByUserID,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_EventCategories_Users_DeletedByUserID",
                        column: x => x.DeletedByUserID,
                        principalTable: "Users",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_EventCategories_Users_UpdatedByUserID",
                        column: x => x.UpdatedByUserID,
                        principalTable: "Users",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "Expenses",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "varchar(20)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedByUserID = table.Column<int>(type: "int", nullable: false),
                    UpdatedByUserID = table.Column<int>(type: "int", nullable: true),
                    DeletedByUserID = table.Column<int>(type: "int", nullable: true),
                    CreatedDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedDateTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Expenses", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Expenses_Users_CreatedByUserID",
                        column: x => x.CreatedByUserID,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Expenses_Users_DeletedByUserID",
                        column: x => x.DeletedByUserID,
                        principalTable: "Users",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_Expenses_Users_UpdatedByUserID",
                        column: x => x.UpdatedByUserID,
                        principalTable: "Users",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "PaymentModes",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "varchar(40)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedByUserID = table.Column<int>(type: "int", nullable: false),
                    UpdatedByUserID = table.Column<int>(type: "int", nullable: true),
                    DeletedByUserID = table.Column<int>(type: "int", nullable: true),
                    CreatedDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedDateTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentModes", x => x.ID);
                    table.ForeignKey(
                        name: "FK_PaymentModes_Users_CreatedByUserID",
                        column: x => x.CreatedByUserID,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_PaymentModes_Users_DeletedByUserID",
                        column: x => x.DeletedByUserID,
                        principalTable: "Users",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_PaymentModes_Users_UpdatedByUserID",
                        column: x => x.UpdatedByUserID,
                        principalTable: "Users",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "ReservationImages",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReservationID = table.Column<int>(type: "int", nullable: false),
                    ImagePath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedByUserID = table.Column<int>(type: "int", nullable: false),
                    UpdatedByUserID = table.Column<int>(type: "int", nullable: true),
                    DeletedByUserID = table.Column<int>(type: "int", nullable: true),
                    CreatedDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedDateTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReservationImages", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ReservationImages_Users_CreatedByUserID",
                        column: x => x.CreatedByUserID,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_ReservationImages_Users_DeletedByUserID",
                        column: x => x.DeletedByUserID,
                        principalTable: "Users",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_ReservationImages_Users_UpdatedByUserID",
                        column: x => x.UpdatedByUserID,
                        principalTable: "Users",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "ReservationStatuses",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "varchar(40)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedByUserID = table.Column<int>(type: "int", nullable: false),
                    UpdatedByUserID = table.Column<int>(type: "int", nullable: true),
                    DeletedByUserID = table.Column<int>(type: "int", nullable: true),
                    CreatedDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedDateTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReservationStatuses", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ReservationStatuses_Users_CreatedByUserID",
                        column: x => x.CreatedByUserID,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_ReservationStatuses_Users_DeletedByUserID",
                        column: x => x.DeletedByUserID,
                        principalTable: "Users",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_ReservationStatuses_Users_UpdatedByUserID",
                        column: x => x.UpdatedByUserID,
                        principalTable: "Users",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "ReservationTickets",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TicketID = table.Column<int>(type: "int", nullable: false),
                    ReservationID = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedByUserID = table.Column<int>(type: "int", nullable: false),
                    UpdatedByUserID = table.Column<int>(type: "int", nullable: true),
                    DeletedByUserID = table.Column<int>(type: "int", nullable: true),
                    CreatedDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedDateTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReservationTickets", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ReservationTickets_Users_CreatedByUserID",
                        column: x => x.CreatedByUserID,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_ReservationTickets_Users_DeletedByUserID",
                        column: x => x.DeletedByUserID,
                        principalTable: "Users",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_ReservationTickets_Users_UpdatedByUserID",
                        column: x => x.UpdatedByUserID,
                        principalTable: "Users",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "Venues",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "varchar(50)", nullable: false),
                    Address = table.Column<string>(type: "varchar(300)", nullable: false),
                    ImagePath = table.Column<string>(type: "varchar(300)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedByUserID = table.Column<int>(type: "int", nullable: false),
                    UpdatedByUserID = table.Column<int>(type: "int", nullable: true),
                    DeletedByUserID = table.Column<int>(type: "int", nullable: true),
                    CreatedDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedDateTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Venues", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Venues_Users_CreatedByUserID",
                        column: x => x.CreatedByUserID,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Venues_Users_DeletedByUserID",
                        column: x => x.DeletedByUserID,
                        principalTable: "Users",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_Venues_Users_UpdatedByUserID",
                        column: x => x.UpdatedByUserID,
                        principalTable: "Users",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "CategoryEventCategory",
                columns: table => new
                {
                    CategoriesID = table.Column<int>(type: "int", nullable: false),
                    EventCategoriesListID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryEventCategory", x => new { x.CategoriesID, x.EventCategoriesListID });
                    table.ForeignKey(
                        name: "FK_CategoryEventCategory_Categories_CategoriesID",
                        column: x => x.CategoriesID,
                        principalTable: "Categories",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_CategoryEventCategory_EventCategories_EventCategoriesListID",
                        column: x => x.EventCategoriesListID,
                        principalTable: "EventCategories",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "Auditoriums",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "varchar(50)", nullable: false),
                    ImagePath = table.Column<string>(type: "varchar(300)", nullable: true),
                    VenueID = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedByUserID = table.Column<int>(type: "int", nullable: false),
                    UpdatedByUserID = table.Column<int>(type: "int", nullable: true),
                    DeletedByUserID = table.Column<int>(type: "int", nullable: true),
                    CreatedDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedDateTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Auditoriums", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Auditoriums_Users_CreatedByUserID",
                        column: x => x.CreatedByUserID,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Auditoriums_Users_DeletedByUserID",
                        column: x => x.DeletedByUserID,
                        principalTable: "Users",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_Auditoriums_Users_UpdatedByUserID",
                        column: x => x.UpdatedByUserID,
                        principalTable: "Users",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_Auditoriums_Venues_VenueID",
                        column: x => x.VenueID,
                        principalTable: "Venues",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "Events",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "varchar(50)", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsGeneral = table.Column<bool>(type: "bit", nullable: false),
                    IsEnded = table.Column<bool>(type: "bit", nullable: false),
                    ImagePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AuditoriumID = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedByUserID = table.Column<int>(type: "int", nullable: false),
                    UpdatedByUserID = table.Column<int>(type: "int", nullable: true),
                    DeletedByUserID = table.Column<int>(type: "int", nullable: true),
                    CreatedDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedDateTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Events", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Events_Auditoriums_AuditoriumID",
                        column: x => x.AuditoriumID,
                        principalTable: "Auditoriums",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Events_Users_CreatedByUserID",
                        column: x => x.CreatedByUserID,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Events_Users_DeletedByUserID",
                        column: x => x.DeletedByUserID,
                        principalTable: "Users",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_Events_Users_UpdatedByUserID",
                        column: x => x.UpdatedByUserID,
                        principalTable: "Users",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "Rows",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RowLabel = table.Column<string>(type: "varchar(1)", nullable: false),
                    AuditoriumID = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedByUserID = table.Column<int>(type: "int", nullable: false),
                    UpdatedByUserID = table.Column<int>(type: "int", nullable: true),
                    DeletedByUserID = table.Column<int>(type: "int", nullable: true),
                    CreatedDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedDateTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rows", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Rows_Auditoriums_AuditoriumID",
                        column: x => x.AuditoriumID,
                        principalTable: "Auditoriums",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Rows_Users_CreatedByUserID",
                        column: x => x.CreatedByUserID,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Rows_Users_DeletedByUserID",
                        column: x => x.DeletedByUserID,
                        principalTable: "Users",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_Rows_Users_UpdatedByUserID",
                        column: x => x.UpdatedByUserID,
                        principalTable: "Users",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "EventEventCategory",
                columns: table => new
                {
                    EventCategoriesID = table.Column<int>(type: "int", nullable: false),
                    EventsID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventEventCategory", x => new { x.EventCategoriesID, x.EventsID });
                    table.ForeignKey(
                        name: "FK_EventEventCategory_EventCategories_EventCategoriesID",
                        column: x => x.EventCategoriesID,
                        principalTable: "EventCategories",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_EventEventCategory_Events_EventsID",
                        column: x => x.EventsID,
                        principalTable: "Events",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "Shows",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EventID = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Time = table.Column<TimeSpan>(type: "time", nullable: false),
                    IsEnded = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedByUserID = table.Column<int>(type: "int", nullable: false),
                    UpdatedByUserID = table.Column<int>(type: "int", nullable: true),
                    DeletedByUserID = table.Column<int>(type: "int", nullable: true),
                    CreatedDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedDateTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shows", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Shows_Events_EventID",
                        column: x => x.EventID,
                        principalTable: "Events",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Shows_Users_CreatedByUserID",
                        column: x => x.CreatedByUserID,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Shows_Users_DeletedByUserID",
                        column: x => x.DeletedByUserID,
                        principalTable: "Users",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_Shows_Users_UpdatedByUserID",
                        column: x => x.UpdatedByUserID,
                        principalTable: "Users",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "Columns",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ColumnNumber = table.Column<int>(type: "int", nullable: false),
                    RowID = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedByUserID = table.Column<int>(type: "int", nullable: false),
                    UpdatedByUserID = table.Column<int>(type: "int", nullable: true),
                    DeletedByUserID = table.Column<int>(type: "int", nullable: true),
                    CreatedDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedDateTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Columns", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Columns_Rows_RowID",
                        column: x => x.RowID,
                        principalTable: "Rows",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Columns_Users_CreatedByUserID",
                        column: x => x.CreatedByUserID,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Columns_Users_DeletedByUserID",
                        column: x => x.DeletedByUserID,
                        principalTable: "Users",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_Columns_Users_UpdatedByUserID",
                        column: x => x.UpdatedByUserID,
                        principalTable: "Users",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "Reservations",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TicketNumber = table.Column<string>(type: "varchar(20)", nullable: false),
                    CustomerName = table.Column<string>(type: "varchar(41)", nullable: false),
                    CustomerPhone = table.Column<string>(type: "varchar(20)", nullable: false),
                    CostPrice = table.Column<double>(type: "float", nullable: false),
                    Discount = table.Column<double>(type: "float", nullable: false),
                    SalePrice = table.Column<double>(type: "float", nullable: false),
                    AmountPaid = table.Column<double>(type: "float", nullable: false),
                    AmountRemaining = table.Column<double>(type: "float", nullable: false),
                    IsFullyPaid = table.Column<bool>(type: "bit", nullable: false),
                    TransactionID = table.Column<string>(type: "varchar(50)", nullable: true),
                    BookingDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ShowID = table.Column<int>(type: "int", nullable: false),
                    PaymentModeID = table.Column<int>(type: "int", nullable: false),
                    ReservationStatusID = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedByUserID = table.Column<int>(type: "int", nullable: false),
                    UpdatedByUserID = table.Column<int>(type: "int", nullable: true),
                    DeletedByUserID = table.Column<int>(type: "int", nullable: true),
                    CreatedDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedDateTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reservations", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Reservations_PaymentModes_PaymentModeID",
                        column: x => x.PaymentModeID,
                        principalTable: "PaymentModes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Reservations_ReservationStatuses_ReservationStatusID",
                        column: x => x.ReservationStatusID,
                        principalTable: "ReservationStatuses",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Reservations_Shows_ShowID",
                        column: x => x.ShowID,
                        principalTable: "Shows",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Reservations_Users_CreatedByUserID",
                        column: x => x.CreatedByUserID,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Reservations_Users_DeletedByUserID",
                        column: x => x.DeletedByUserID,
                        principalTable: "Users",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_Reservations_Users_UpdatedByUserID",
                        column: x => x.UpdatedByUserID,
                        principalTable: "Users",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "ShowExpenses",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EventID = table.Column<int>(type: "int", nullable: false),
                    ShowID = table.Column<int>(type: "int", nullable: false),
                    ExpenseID = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Amount = table.Column<double>(type: "float", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedByUserID = table.Column<int>(type: "int", nullable: false),
                    UpdatedByUserID = table.Column<int>(type: "int", nullable: true),
                    DeletedByUserID = table.Column<int>(type: "int", nullable: true),
                    CreatedDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedDateTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShowExpenses", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ShowExpenses_Events_EventID",
                        column: x => x.EventID,
                        principalTable: "Events",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_ShowExpenses_Expenses_ExpenseID",
                        column: x => x.ExpenseID,
                        principalTable: "Expenses",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_ShowExpenses_Shows_ShowID",
                        column: x => x.ShowID,
                        principalTable: "Shows",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_ShowExpenses_Users_CreatedByUserID",
                        column: x => x.CreatedByUserID,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_ShowExpenses_Users_DeletedByUserID",
                        column: x => x.DeletedByUserID,
                        principalTable: "Users",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_ShowExpenses_Users_UpdatedByUserID",
                        column: x => x.UpdatedByUserID,
                        principalTable: "Users",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "Seats",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SeatNumber = table.Column<int>(type: "int", nullable: false),
                    ColumnID = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedByUserID = table.Column<int>(type: "int", nullable: false),
                    UpdatedByUserID = table.Column<int>(type: "int", nullable: true),
                    DeletedByUserID = table.Column<int>(type: "int", nullable: true),
                    CreatedDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedDateTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Seats", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Seats_Columns_ColumnID",
                        column: x => x.ColumnID,
                        principalTable: "Columns",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Seats_Users_CreatedByUserID",
                        column: x => x.CreatedByUserID,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Seats_Users_DeletedByUserID",
                        column: x => x.DeletedByUserID,
                        principalTable: "Users",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_Seats_Users_UpdatedByUserID",
                        column: x => x.UpdatedByUserID,
                        principalTable: "Users",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "ReservationReservationImage",
                columns: table => new
                {
                    ReservationImagesID = table.Column<int>(type: "int", nullable: false),
                    ReservationsID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReservationReservationImage", x => new { x.ReservationImagesID, x.ReservationsID });
                    table.ForeignKey(
                        name: "FK_ReservationReservationImage_ReservationImages_ReservationImagesID",
                        column: x => x.ReservationImagesID,
                        principalTable: "ReservationImages",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_ReservationReservationImage_Reservations_ReservationsID",
                        column: x => x.ReservationsID,
                        principalTable: "Reservations",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "ReservationReservationTicket",
                columns: table => new
                {
                    ReservationTicketsID = table.Column<int>(type: "int", nullable: false),
                    ReservationsID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReservationReservationTicket", x => new { x.ReservationTicketsID, x.ReservationsID });
                    table.ForeignKey(
                        name: "FK_ReservationReservationTicket_ReservationTickets_ReservationTicketsID",
                        column: x => x.ReservationTicketsID,
                        principalTable: "ReservationTickets",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_ReservationReservationTicket_Reservations_ReservationsID",
                        column: x => x.ReservationsID,
                        principalTable: "Reservations",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "Tickets",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ShowID = table.Column<int>(type: "int", nullable: false),
                    SeatID = table.Column<int>(type: "int", nullable: false),
                    CostPrice = table.Column<double>(type: "float", nullable: false),
                    Discount = table.Column<double>(type: "float", nullable: false),
                    SalePrice = table.Column<double>(type: "float", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedByUserID = table.Column<int>(type: "int", nullable: false),
                    UpdatedByUserID = table.Column<int>(type: "int", nullable: true),
                    DeletedByUserID = table.Column<int>(type: "int", nullable: true),
                    CreatedDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedDateTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tickets", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Tickets_Seats_SeatID",
                        column: x => x.SeatID,
                        principalTable: "Seats",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Tickets_Shows_ShowID",
                        column: x => x.ShowID,
                        principalTable: "Shows",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Tickets_Users_CreatedByUserID",
                        column: x => x.CreatedByUserID,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Tickets_Users_DeletedByUserID",
                        column: x => x.DeletedByUserID,
                        principalTable: "Users",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_Tickets_Users_UpdatedByUserID",
                        column: x => x.UpdatedByUserID,
                        principalTable: "Users",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "ReservationTicketTicket",
                columns: table => new
                {
                    ReservationTicketsID = table.Column<int>(type: "int", nullable: false),
                    TicketsID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReservationTicketTicket", x => new { x.ReservationTicketsID, x.TicketsID });
                    table.ForeignKey(
                        name: "FK_ReservationTicketTicket_ReservationTickets_ReservationTicketsID",
                        column: x => x.ReservationTicketsID,
                        principalTable: "ReservationTickets",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_ReservationTicketTicket_Tickets_TicketsID",
                        column: x => x.TicketsID,
                        principalTable: "Tickets",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "ID", "IsDeleted", "Title" },
                values: new object[,]
                {
                    { 1, false, "Super Admin" },
                    { 2, false, "Admin" },
                    { 3, false, "Moderator" },
                    { 4, false, "User" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "ID", "ContactNumber", "CreatedByUserID", "CreatedDateTime", "DeletedByUserID", "DeletedDateTime", "Email", "FirstName", "ImagePath", "IsActive", "IsDeleted", "IsVerified", "LastName", "Password", "RoleID", "UpdatedByUserID", "UpdatedDateTime", "UserToken" },
                values: new object[] { 1, "+923312541767", null, new DateTime(2025, 7, 2, 13, 18, 4, 298, DateTimeKind.Utc).AddTicks(8811), null, null, "sa@eventland.pk", "Super", null, true, false, true, "Admin", "6FkLTXY5CrK8ccZIBV6HAibnMhfq44wfgPMuzSxJjQ4=", 1, null, null, null });

            migrationBuilder.InsertData(
                table: "PaymentModes",
                columns: new[] { "ID", "CreatedByUserID", "CreatedDateTime", "DeletedByUserID", "DeletedDateTime", "IsDeleted", "Title", "UpdatedByUserID", "UpdatedDateTime" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2025, 7, 2, 13, 18, 4, 545, DateTimeKind.Utc).AddTicks(7513), null, null, false, "Cash", null, null },
                    { 2, 1, new DateTime(2025, 7, 2, 13, 18, 4, 545, DateTimeKind.Utc).AddTicks(7517), null, null, false, "Easypaisa", null, null },
                    { 3, 1, new DateTime(2025, 7, 2, 13, 18, 4, 545, DateTimeKind.Utc).AddTicks(7518), null, null, false, "Jazzcash", null, null },
                    { 4, 1, new DateTime(2025, 7, 2, 13, 18, 4, 545, DateTimeKind.Utc).AddTicks(7519), null, null, false, "Bank Transfer", null, null }
                });

            migrationBuilder.InsertData(
                table: "ReservationStatuses",
                columns: new[] { "ID", "CreatedByUserID", "CreatedDateTime", "DeletedByUserID", "DeletedDateTime", "IsDeleted", "Title", "UpdatedByUserID", "UpdatedDateTime" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2025, 7, 2, 13, 18, 4, 545, DateTimeKind.Utc).AddTicks(7670), null, null, false, "Arrival Pending", null, null },
                    { 2, 1, new DateTime(2025, 7, 2, 13, 18, 4, 545, DateTimeKind.Utc).AddTicks(7673), null, null, false, "Not Attended", null, null },
                    { 3, 1, new DateTime(2025, 7, 2, 13, 18, 4, 545, DateTimeKind.Utc).AddTicks(7674), null, null, false, "Closed", null, null },
                    { 4, 1, new DateTime(2025, 7, 2, 13, 18, 4, 545, DateTimeKind.Utc).AddTicks(7675), null, null, false, "Refunded", null, null }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "ID", "ContactNumber", "CreatedByUserID", "CreatedDateTime", "DeletedByUserID", "DeletedDateTime", "Email", "FirstName", "ImagePath", "IsActive", "IsDeleted", "IsVerified", "LastName", "Password", "RoleID", "UpdatedByUserID", "UpdatedDateTime", "UserToken" },
                values: new object[,]
                {
                    { 2, "+923312541767", 1, new DateTime(2025, 7, 2, 13, 18, 4, 396, DateTimeKind.Utc).AddTicks(3665), null, null, "qa.ansari80@eventland.pk", "Qamar", null, true, false, true, "Ansari", "Lkej/d5kATMjqn/M3Ku6xHURlNX+8UjTX1IGD38nWgA=", 2, null, null, null },
                    { 3, "+923162361582", 1, new DateTime(2025, 7, 2, 13, 18, 4, 469, DateTimeKind.Utc).AddTicks(6339), null, null, "stayyabahmed85@eventland.pk", "Tayyab", null, true, false, true, "Ahmed", "0ihPPTvs1St6AoZbcGjX2OBvl9iCyjRbs3xu/NvdeII=", 2, null, null, null },
                    { 4, "+923091265921", 1, new DateTime(2025, 7, 2, 13, 18, 4, 545, DateTimeKind.Utc).AddTicks(6844), null, null, "anasali4772@eventland.pk", "Anas", null, true, false, true, "Ali", "mNIvAtqhtWlJG6AG2sfaPWxKC1i9huTTqVcxkhD5qBM=", 2, null, null, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Auditoriums_CreatedByUserID",
                table: "Auditoriums",
                column: "CreatedByUserID");

            migrationBuilder.CreateIndex(
                name: "IX_Auditoriums_DeletedByUserID",
                table: "Auditoriums",
                column: "DeletedByUserID");

            migrationBuilder.CreateIndex(
                name: "IX_Auditoriums_UpdatedByUserID",
                table: "Auditoriums",
                column: "UpdatedByUserID");

            migrationBuilder.CreateIndex(
                name: "IX_Auditoriums_VenueID",
                table: "Auditoriums",
                column: "VenueID");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_CreatedByUserID",
                table: "Categories",
                column: "CreatedByUserID");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_DeletedByUserID",
                table: "Categories",
                column: "DeletedByUserID");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_UpdatedByUserID",
                table: "Categories",
                column: "UpdatedByUserID");

            migrationBuilder.CreateIndex(
                name: "IX_CategoryEventCategory_EventCategoriesListID",
                table: "CategoryEventCategory",
                column: "EventCategoriesListID");

            migrationBuilder.CreateIndex(
                name: "IX_Columns_CreatedByUserID",
                table: "Columns",
                column: "CreatedByUserID");

            migrationBuilder.CreateIndex(
                name: "IX_Columns_DeletedByUserID",
                table: "Columns",
                column: "DeletedByUserID");

            migrationBuilder.CreateIndex(
                name: "IX_Columns_RowID",
                table: "Columns",
                column: "RowID");

            migrationBuilder.CreateIndex(
                name: "IX_Columns_UpdatedByUserID",
                table: "Columns",
                column: "UpdatedByUserID");

            migrationBuilder.CreateIndex(
                name: "IX_EventCategories_CreatedByUserID",
                table: "EventCategories",
                column: "CreatedByUserID");

            migrationBuilder.CreateIndex(
                name: "IX_EventCategories_DeletedByUserID",
                table: "EventCategories",
                column: "DeletedByUserID");

            migrationBuilder.CreateIndex(
                name: "IX_EventCategories_UpdatedByUserID",
                table: "EventCategories",
                column: "UpdatedByUserID");

            migrationBuilder.CreateIndex(
                name: "IX_EventEventCategory_EventsID",
                table: "EventEventCategory",
                column: "EventsID");

            migrationBuilder.CreateIndex(
                name: "IX_Events_AuditoriumID",
                table: "Events",
                column: "AuditoriumID");

            migrationBuilder.CreateIndex(
                name: "IX_Events_CreatedByUserID",
                table: "Events",
                column: "CreatedByUserID");

            migrationBuilder.CreateIndex(
                name: "IX_Events_DeletedByUserID",
                table: "Events",
                column: "DeletedByUserID");

            migrationBuilder.CreateIndex(
                name: "IX_Events_UpdatedByUserID",
                table: "Events",
                column: "UpdatedByUserID");

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_CreatedByUserID",
                table: "Expenses",
                column: "CreatedByUserID");

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_DeletedByUserID",
                table: "Expenses",
                column: "DeletedByUserID");

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_UpdatedByUserID",
                table: "Expenses",
                column: "UpdatedByUserID");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentModes_CreatedByUserID",
                table: "PaymentModes",
                column: "CreatedByUserID");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentModes_DeletedByUserID",
                table: "PaymentModes",
                column: "DeletedByUserID");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentModes_UpdatedByUserID",
                table: "PaymentModes",
                column: "UpdatedByUserID");

            migrationBuilder.CreateIndex(
                name: "IX_ReservationImages_CreatedByUserID",
                table: "ReservationImages",
                column: "CreatedByUserID");

            migrationBuilder.CreateIndex(
                name: "IX_ReservationImages_DeletedByUserID",
                table: "ReservationImages",
                column: "DeletedByUserID");

            migrationBuilder.CreateIndex(
                name: "IX_ReservationImages_UpdatedByUserID",
                table: "ReservationImages",
                column: "UpdatedByUserID");

            migrationBuilder.CreateIndex(
                name: "IX_ReservationReservationImage_ReservationsID",
                table: "ReservationReservationImage",
                column: "ReservationsID");

            migrationBuilder.CreateIndex(
                name: "IX_ReservationReservationTicket_ReservationsID",
                table: "ReservationReservationTicket",
                column: "ReservationsID");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_CreatedByUserID",
                table: "Reservations",
                column: "CreatedByUserID");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_DeletedByUserID",
                table: "Reservations",
                column: "DeletedByUserID");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_PaymentModeID",
                table: "Reservations",
                column: "PaymentModeID");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_ReservationStatusID",
                table: "Reservations",
                column: "ReservationStatusID");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_ShowID",
                table: "Reservations",
                column: "ShowID");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_UpdatedByUserID",
                table: "Reservations",
                column: "UpdatedByUserID");

            migrationBuilder.CreateIndex(
                name: "IX_ReservationStatuses_CreatedByUserID",
                table: "ReservationStatuses",
                column: "CreatedByUserID");

            migrationBuilder.CreateIndex(
                name: "IX_ReservationStatuses_DeletedByUserID",
                table: "ReservationStatuses",
                column: "DeletedByUserID");

            migrationBuilder.CreateIndex(
                name: "IX_ReservationStatuses_UpdatedByUserID",
                table: "ReservationStatuses",
                column: "UpdatedByUserID");

            migrationBuilder.CreateIndex(
                name: "IX_ReservationTickets_CreatedByUserID",
                table: "ReservationTickets",
                column: "CreatedByUserID");

            migrationBuilder.CreateIndex(
                name: "IX_ReservationTickets_DeletedByUserID",
                table: "ReservationTickets",
                column: "DeletedByUserID");

            migrationBuilder.CreateIndex(
                name: "IX_ReservationTickets_UpdatedByUserID",
                table: "ReservationTickets",
                column: "UpdatedByUserID");

            migrationBuilder.CreateIndex(
                name: "IX_ReservationTicketTicket_TicketsID",
                table: "ReservationTicketTicket",
                column: "TicketsID");

            migrationBuilder.CreateIndex(
                name: "IX_Rows_AuditoriumID",
                table: "Rows",
                column: "AuditoriumID");

            migrationBuilder.CreateIndex(
                name: "IX_Rows_CreatedByUserID",
                table: "Rows",
                column: "CreatedByUserID");

            migrationBuilder.CreateIndex(
                name: "IX_Rows_DeletedByUserID",
                table: "Rows",
                column: "DeletedByUserID");

            migrationBuilder.CreateIndex(
                name: "IX_Rows_UpdatedByUserID",
                table: "Rows",
                column: "UpdatedByUserID");

            migrationBuilder.CreateIndex(
                name: "IX_Seats_ColumnID",
                table: "Seats",
                column: "ColumnID");

            migrationBuilder.CreateIndex(
                name: "IX_Seats_CreatedByUserID",
                table: "Seats",
                column: "CreatedByUserID");

            migrationBuilder.CreateIndex(
                name: "IX_Seats_DeletedByUserID",
                table: "Seats",
                column: "DeletedByUserID");

            migrationBuilder.CreateIndex(
                name: "IX_Seats_UpdatedByUserID",
                table: "Seats",
                column: "UpdatedByUserID");

            migrationBuilder.CreateIndex(
                name: "IX_ShowExpenses_CreatedByUserID",
                table: "ShowExpenses",
                column: "CreatedByUserID");

            migrationBuilder.CreateIndex(
                name: "IX_ShowExpenses_DeletedByUserID",
                table: "ShowExpenses",
                column: "DeletedByUserID");

            migrationBuilder.CreateIndex(
                name: "IX_ShowExpenses_EventID",
                table: "ShowExpenses",
                column: "EventID");

            migrationBuilder.CreateIndex(
                name: "IX_ShowExpenses_ExpenseID",
                table: "ShowExpenses",
                column: "ExpenseID");

            migrationBuilder.CreateIndex(
                name: "IX_ShowExpenses_ShowID",
                table: "ShowExpenses",
                column: "ShowID");

            migrationBuilder.CreateIndex(
                name: "IX_ShowExpenses_UpdatedByUserID",
                table: "ShowExpenses",
                column: "UpdatedByUserID");

            migrationBuilder.CreateIndex(
                name: "IX_Shows_CreatedByUserID",
                table: "Shows",
                column: "CreatedByUserID");

            migrationBuilder.CreateIndex(
                name: "IX_Shows_DeletedByUserID",
                table: "Shows",
                column: "DeletedByUserID");

            migrationBuilder.CreateIndex(
                name: "IX_Shows_EventID",
                table: "Shows",
                column: "EventID");

            migrationBuilder.CreateIndex(
                name: "IX_Shows_UpdatedByUserID",
                table: "Shows",
                column: "UpdatedByUserID");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_CreatedByUserID",
                table: "Tickets",
                column: "CreatedByUserID");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_DeletedByUserID",
                table: "Tickets",
                column: "DeletedByUserID");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_SeatID",
                table: "Tickets",
                column: "SeatID");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_ShowID",
                table: "Tickets",
                column: "ShowID");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_UpdatedByUserID",
                table: "Tickets",
                column: "UpdatedByUserID");

            migrationBuilder.CreateIndex(
                name: "IX_Users_CreatedByUserID",
                table: "Users",
                column: "CreatedByUserID");

            migrationBuilder.CreateIndex(
                name: "IX_Users_DeletedByUserID",
                table: "Users",
                column: "DeletedByUserID");

            migrationBuilder.CreateIndex(
                name: "IX_Users_RoleID",
                table: "Users",
                column: "RoleID");

            migrationBuilder.CreateIndex(
                name: "IX_Users_UpdatedByUserID",
                table: "Users",
                column: "UpdatedByUserID");

            migrationBuilder.CreateIndex(
                name: "IX_Venues_CreatedByUserID",
                table: "Venues",
                column: "CreatedByUserID");

            migrationBuilder.CreateIndex(
                name: "IX_Venues_DeletedByUserID",
                table: "Venues",
                column: "DeletedByUserID");

            migrationBuilder.CreateIndex(
                name: "IX_Venues_UpdatedByUserID",
                table: "Venues",
                column: "UpdatedByUserID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CategoryEventCategory");

            migrationBuilder.DropTable(
                name: "EventEventCategory");

            migrationBuilder.DropTable(
                name: "ReservationReservationImage");

            migrationBuilder.DropTable(
                name: "ReservationReservationTicket");

            migrationBuilder.DropTable(
                name: "ReservationTicketTicket");

            migrationBuilder.DropTable(
                name: "ShowExpenses");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "EventCategories");

            migrationBuilder.DropTable(
                name: "ReservationImages");

            migrationBuilder.DropTable(
                name: "Reservations");

            migrationBuilder.DropTable(
                name: "ReservationTickets");

            migrationBuilder.DropTable(
                name: "Tickets");

            migrationBuilder.DropTable(
                name: "Expenses");

            migrationBuilder.DropTable(
                name: "PaymentModes");

            migrationBuilder.DropTable(
                name: "ReservationStatuses");

            migrationBuilder.DropTable(
                name: "Seats");

            migrationBuilder.DropTable(
                name: "Shows");

            migrationBuilder.DropTable(
                name: "Columns");

            migrationBuilder.DropTable(
                name: "Events");

            migrationBuilder.DropTable(
                name: "Rows");

            migrationBuilder.DropTable(
                name: "Auditoriums");

            migrationBuilder.DropTable(
                name: "Venues");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Roles");
        }
    }
}
