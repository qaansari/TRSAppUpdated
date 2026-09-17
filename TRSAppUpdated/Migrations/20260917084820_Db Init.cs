using System;
using Microsoft.EntityFrameworkCore.Metadata;
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
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(type: "varchar(20)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.ID);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Auditoriums",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(type: "varchar(50)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ImagePath = table.Column<string>(type: "varchar(300)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NoOfRows = table.Column<int>(type: "int", nullable: false),
                    StartingRow = table.Column<string>(type: "varchar(1)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Alignment = table.Column<string>(type: "varchar(5)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    VenueID = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedByUserID = table.Column<int>(type: "int", nullable: false),
                    UpdatedByUserID = table.Column<int>(type: "int", nullable: true),
                    DeletedByUserID = table.Column<int>(type: "int", nullable: true),
                    CreatedDateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedDateTime = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    DeletedDateTime = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Auditoriums", x => x.ID);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(type: "varchar(20)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedByUserID = table.Column<int>(type: "int", nullable: false),
                    UpdatedByUserID = table.Column<int>(type: "int", nullable: true),
                    DeletedByUserID = table.Column<int>(type: "int", nullable: true),
                    CreatedDateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedDateTime = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    DeletedDateTime = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.ID);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Cities",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(type: "varchar(40)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CountryID = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedByUserID = table.Column<int>(type: "int", nullable: false),
                    UpdatedByUserID = table.Column<int>(type: "int", nullable: true),
                    DeletedByUserID = table.Column<int>(type: "int", nullable: true),
                    CreatedDateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedDateTime = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    DeletedDateTime = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cities", x => x.ID);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Columns",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ColumnNumber = table.Column<int>(type: "int", nullable: false),
                    RowID = table.Column<int>(type: "int", nullable: false),
                    CreatedByUserID = table.Column<int>(type: "int", nullable: false),
                    CreatedDateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Columns", x => x.ID);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Countries",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(type: "varchar(40)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DialingCode = table.Column<string>(type: "varchar(4)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Flag = table.Column<string>(type: "varchar(50)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedByUserID = table.Column<int>(type: "int", nullable: false),
                    UpdatedByUserID = table.Column<int>(type: "int", nullable: true),
                    DeletedByUserID = table.Column<int>(type: "int", nullable: true),
                    CreatedDateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedDateTime = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    DeletedDateTime = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Countries", x => x.ID);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    FirstName = table.Column<string>(type: "varchar(20)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    LastName = table.Column<string>(type: "varchar(20)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ContactNumber = table.Column<string>(type: "varchar(20)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Email = table.Column<string>(type: "varchar(320)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Password = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RoleID = table.Column<int>(type: "int", nullable: false),
                    CountryID = table.Column<int>(type: "int", nullable: true),
                    ImagePath = table.Column<string>(type: "varchar(300)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserToken = table.Column<string>(type: "varchar(68)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TokenExpiryTime = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsVerified = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedByUserID = table.Column<int>(type: "int", nullable: true),
                    UpdatedByUserID = table.Column<int>(type: "int", nullable: true),
                    DeletedByUserID = table.Column<int>(type: "int", nullable: true),
                    CreatedDateTime = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UpdatedDateTime = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    DeletedDateTime = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Users_Countries_CountryID",
                        column: x => x.CountryID,
                        principalTable: "Countries",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_Users_Roles_RoleID",
                        column: x => x.RoleID,
                        principalTable: "Roles",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
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
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "EventStatuses",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(type: "varchar(40)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedByUserID = table.Column<int>(type: "int", nullable: false),
                    UpdatedByUserID = table.Column<int>(type: "int", nullable: true),
                    DeletedByUserID = table.Column<int>(type: "int", nullable: true),
                    CreatedDateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedDateTime = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    DeletedDateTime = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventStatuses", x => x.ID);
                    table.ForeignKey(
                        name: "FK_EventStatuses_Users_CreatedByUserID",
                        column: x => x.CreatedByUserID,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventStatuses_Users_DeletedByUserID",
                        column: x => x.DeletedByUserID,
                        principalTable: "Users",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_EventStatuses_Users_UpdatedByUserID",
                        column: x => x.UpdatedByUserID,
                        principalTable: "Users",
                        principalColumn: "ID");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Expenses",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(type: "varchar(20)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedByUserID = table.Column<int>(type: "int", nullable: false),
                    UpdatedByUserID = table.Column<int>(type: "int", nullable: true),
                    DeletedByUserID = table.Column<int>(type: "int", nullable: true),
                    CreatedDateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedDateTime = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    DeletedDateTime = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Expenses", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Expenses_Users_CreatedByUserID",
                        column: x => x.CreatedByUserID,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
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
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "NoteTypes",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(type: "varchar(20)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedByUserID = table.Column<int>(type: "int", nullable: false),
                    UpdatedByUserID = table.Column<int>(type: "int", nullable: true),
                    DeletedByUserID = table.Column<int>(type: "int", nullable: true),
                    CreatedDateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedDateTime = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    DeletedDateTime = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NoteTypes", x => x.ID);
                    table.ForeignKey(
                        name: "FK_NoteTypes_Users_CreatedByUserID",
                        column: x => x.CreatedByUserID,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NoteTypes_Users_DeletedByUserID",
                        column: x => x.DeletedByUserID,
                        principalTable: "Users",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_NoteTypes_Users_UpdatedByUserID",
                        column: x => x.UpdatedByUserID,
                        principalTable: "Users",
                        principalColumn: "ID");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "PaymentModes",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(type: "varchar(40)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedByUserID = table.Column<int>(type: "int", nullable: false),
                    UpdatedByUserID = table.Column<int>(type: "int", nullable: true),
                    DeletedByUserID = table.Column<int>(type: "int", nullable: true),
                    CreatedDateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedDateTime = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    DeletedDateTime = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentModes", x => x.ID);
                    table.ForeignKey(
                        name: "FK_PaymentModes_Users_CreatedByUserID",
                        column: x => x.CreatedByUserID,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
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
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ReservationStatuses",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(type: "varchar(40)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedByUserID = table.Column<int>(type: "int", nullable: false),
                    UpdatedByUserID = table.Column<int>(type: "int", nullable: true),
                    DeletedByUserID = table.Column<int>(type: "int", nullable: true),
                    CreatedDateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedDateTime = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    DeletedDateTime = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReservationStatuses", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ReservationStatuses_Users_CreatedByUserID",
                        column: x => x.CreatedByUserID,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
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
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Rows",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    RowLabel = table.Column<string>(type: "varchar(1)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AuditoriumID = table.Column<int>(type: "int", nullable: false),
                    CreatedByUserID = table.Column<int>(type: "int", nullable: false),
                    CreatedDateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rows", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Rows_Auditoriums_AuditoriumID",
                        column: x => x.AuditoriumID,
                        principalTable: "Auditoriums",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Rows_Users_CreatedByUserID",
                        column: x => x.CreatedByUserID,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Seats",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    SeatNumber = table.Column<int>(type: "int", nullable: false),
                    ColumnID = table.Column<int>(type: "int", nullable: false),
                    CreatedByUserID = table.Column<int>(type: "int", nullable: false),
                    CreatedDateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Seats", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Seats_Columns_ColumnID",
                        column: x => x.ColumnID,
                        principalTable: "Columns",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Seats_Users_CreatedByUserID",
                        column: x => x.CreatedByUserID,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Venues",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(type: "varchar(50)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Address = table.Column<string>(type: "varchar(300)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CityID = table.Column<int>(type: "int", nullable: false),
                    ImagePath = table.Column<string>(type: "varchar(300)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedByUserID = table.Column<int>(type: "int", nullable: false),
                    UpdatedByUserID = table.Column<int>(type: "int", nullable: true),
                    DeletedByUserID = table.Column<int>(type: "int", nullable: true),
                    CreatedDateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedDateTime = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    DeletedDateTime = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Venues", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Venues_Cities_CityID",
                        column: x => x.CityID,
                        principalTable: "Cities",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Venues_Users_CreatedByUserID",
                        column: x => x.CreatedByUserID,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
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
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Events",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(type: "varchar(50)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    StartDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    Description = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsGeneral = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ImagePath = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AuditoriumID = table.Column<int>(type: "int", nullable: false),
                    EventStatusID = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedByUserID = table.Column<int>(type: "int", nullable: false),
                    UpdatedByUserID = table.Column<int>(type: "int", nullable: true),
                    DeletedByUserID = table.Column<int>(type: "int", nullable: true),
                    CreatedDateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedDateTime = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    DeletedDateTime = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Events", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Events_Auditoriums_AuditoriumID",
                        column: x => x.AuditoriumID,
                        principalTable: "Auditoriums",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Events_EventStatuses_EventStatusID",
                        column: x => x.EventStatusID,
                        principalTable: "EventStatuses",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Events_Users_CreatedByUserID",
                        column: x => x.CreatedByUserID,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
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
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Notes",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Seq = table.Column<int>(type: "int", nullable: false),
                    EnTitle = table.Column<string>(type: "text", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UrduTitle = table.Column<string>(type: "text", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NoteTypeID = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedByUserID = table.Column<int>(type: "int", nullable: false),
                    UpdatedByUserID = table.Column<int>(type: "int", nullable: true),
                    DeletedByUserID = table.Column<int>(type: "int", nullable: true),
                    CreatedDateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedDateTime = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    DeletedDateTime = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notes", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Notes_NoteTypes_NoteTypeID",
                        column: x => x.NoteTypeID,
                        principalTable: "NoteTypes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Notes_Users_CreatedByUserID",
                        column: x => x.CreatedByUserID,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Notes_Users_DeletedByUserID",
                        column: x => x.DeletedByUserID,
                        principalTable: "Users",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_Notes_Users_UpdatedByUserID",
                        column: x => x.UpdatedByUserID,
                        principalTable: "Users",
                        principalColumn: "ID");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "EventCategories",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CategoryID = table.Column<int>(type: "int", nullable: false),
                    EventID = table.Column<int>(type: "int", nullable: false),
                    CreatedByUserID = table.Column<int>(type: "int", nullable: false),
                    CreatedDateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventCategories", x => x.ID);
                    table.ForeignKey(
                        name: "FK_EventCategories_Categories_CategoryID",
                        column: x => x.CategoryID,
                        principalTable: "Categories",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventCategories_Events_EventID",
                        column: x => x.EventID,
                        principalTable: "Events",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventCategories_Users_CreatedByUserID",
                        column: x => x.CreatedByUserID,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Shows",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    EventID = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Time = table.Column<TimeSpan>(type: "time(6)", nullable: false),
                    IsEnded = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedByUserID = table.Column<int>(type: "int", nullable: false),
                    UpdatedByUserID = table.Column<int>(type: "int", nullable: true),
                    DeletedByUserID = table.Column<int>(type: "int", nullable: true),
                    CreatedDateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedDateTime = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    DeletedDateTime = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shows", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Shows_Events_EventID",
                        column: x => x.EventID,
                        principalTable: "Events",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Shows_Users_CreatedByUserID",
                        column: x => x.CreatedByUserID,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
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
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Reservations",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    TicketNumber = table.Column<string>(type: "varchar(20)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CustomerName = table.Column<string>(type: "varchar(41)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CustomerPhone = table.Column<string>(type: "varchar(20)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CostPrice = table.Column<double>(type: "double", nullable: false),
                    Discount = table.Column<double>(type: "double", nullable: false),
                    SalePrice = table.Column<double>(type: "double", nullable: false),
                    AmountPaid = table.Column<double>(type: "double", nullable: false),
                    AmountRemaining = table.Column<double>(type: "double", nullable: false),
                    IsFullyPaid = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    TransactionID = table.Column<string>(type: "varchar(50)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    BookingDateTime = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ShowID = table.Column<int>(type: "int", nullable: false),
                    PaymentModeID = table.Column<int>(type: "int", nullable: false),
                    ReservationStatusID = table.Column<int>(type: "int", nullable: false),
                    ClosedByUserID = table.Column<int>(type: "int", nullable: true),
                    ClosingDateTime = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    IsApproved = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    ActionByUserID = table.Column<int>(type: "int", nullable: true),
                    ActionDateTime = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedByUserID = table.Column<int>(type: "int", nullable: false),
                    UpdatedByUserID = table.Column<int>(type: "int", nullable: true),
                    DeletedByUserID = table.Column<int>(type: "int", nullable: true),
                    CreatedDateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedDateTime = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    DeletedDateTime = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reservations", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Reservations_PaymentModes_PaymentModeID",
                        column: x => x.PaymentModeID,
                        principalTable: "PaymentModes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reservations_ReservationStatuses_ReservationStatusID",
                        column: x => x.ReservationStatusID,
                        principalTable: "ReservationStatuses",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reservations_Shows_ShowID",
                        column: x => x.ShowID,
                        principalTable: "Shows",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reservations_Users_ActionByUserID",
                        column: x => x.ActionByUserID,
                        principalTable: "Users",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_Reservations_Users_ClosedByUserID",
                        column: x => x.ClosedByUserID,
                        principalTable: "Users",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_Reservations_Users_CreatedByUserID",
                        column: x => x.CreatedByUserID,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
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
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ShowExpenses",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    EventID = table.Column<int>(type: "int", nullable: false),
                    ShowID = table.Column<int>(type: "int", nullable: false),
                    ExpenseID = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Amount = table.Column<double>(type: "double", nullable: false),
                    Description = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedByUserID = table.Column<int>(type: "int", nullable: false),
                    UpdatedByUserID = table.Column<int>(type: "int", nullable: true),
                    DeletedByUserID = table.Column<int>(type: "int", nullable: true),
                    CreatedDateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedDateTime = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    DeletedDateTime = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShowExpenses", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ShowExpenses_Events_EventID",
                        column: x => x.EventID,
                        principalTable: "Events",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ShowExpenses_Expenses_ExpenseID",
                        column: x => x.ExpenseID,
                        principalTable: "Expenses",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ShowExpenses_Shows_ShowID",
                        column: x => x.ShowID,
                        principalTable: "Shows",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ShowExpenses_Users_CreatedByUserID",
                        column: x => x.CreatedByUserID,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
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
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Tickets",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ShowID = table.Column<int>(type: "int", nullable: false),
                    SeatID = table.Column<int>(type: "int", nullable: false),
                    CostPrice = table.Column<double>(type: "double", nullable: false),
                    Discount = table.Column<double>(type: "double", nullable: false),
                    SalePrice = table.Column<double>(type: "double", nullable: false),
                    IsReserved = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsRefunded = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsCancelled = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Category = table.Column<string>(type: "varchar(50)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ReservationDateTime = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    RefundDateTime = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CancellationDateTime = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedByUserID = table.Column<int>(type: "int", nullable: false),
                    UpdatedByUserID = table.Column<int>(type: "int", nullable: true),
                    DeletedByUserID = table.Column<int>(type: "int", nullable: true),
                    CreatedDateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedDateTime = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    DeletedDateTime = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tickets", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Tickets_Seats_SeatID",
                        column: x => x.SeatID,
                        principalTable: "Seats",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Tickets_Shows_ShowID",
                        column: x => x.ShowID,
                        principalTable: "Shows",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Tickets_Users_CreatedByUserID",
                        column: x => x.CreatedByUserID,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
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
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ReservationImages",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ReservationID = table.Column<int>(type: "int", nullable: false),
                    ImagePath = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedByUserID = table.Column<int>(type: "int", nullable: false),
                    CreatedDateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReservationImages", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ReservationImages_Reservations_ReservationID",
                        column: x => x.ReservationID,
                        principalTable: "Reservations",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReservationImages_Users_CreatedByUserID",
                        column: x => x.CreatedByUserID,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ReservationTickets",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    TicketID = table.Column<int>(type: "int", nullable: false),
                    ReservationID = table.Column<int>(type: "int", nullable: false),
                    CreatedByUserID = table.Column<int>(type: "int", nullable: false),
                    CreatedDateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReservationTickets", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ReservationTickets_Reservations_ReservationID",
                        column: x => x.ReservationID,
                        principalTable: "Reservations",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReservationTickets_Tickets_TicketID",
                        column: x => x.TicketID,
                        principalTable: "Tickets",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReservationTickets_Users_CreatedByUserID",
                        column: x => x.CreatedByUserID,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

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
                columns: new[] { "ID", "ContactNumber", "CountryID", "CreatedByUserID", "CreatedDateTime", "DeletedByUserID", "DeletedDateTime", "Email", "FirstName", "ImagePath", "IsActive", "IsDeleted", "IsVerified", "LastName", "Password", "RoleID", "TokenExpiryTime", "UpdatedByUserID", "UpdatedDateTime", "UserToken" },
                values: new object[] { 1, "+923312541767", null, null, new DateTime(2026, 9, 17, 13, 48, 17, 493, DateTimeKind.Utc).AddTicks(4015), null, null, "sa@eventland.pk", "Super", null, true, false, true, "Admin", "6FkLTXY5CrK8ccZIBV6HAibnMhfq44wfgPMuzSxJjQ4=", 1, null, null, null, null });

            migrationBuilder.InsertData(
                table: "Countries",
                columns: new[] { "ID", "CreatedByUserID", "CreatedDateTime", "DeletedByUserID", "DeletedDateTime", "DialingCode", "Flag", "IsDeleted", "Title", "UpdatedByUserID", "UpdatedDateTime" },
                values: new object[] { 1, 1, new DateTime(2026, 9, 17, 13, 48, 17, 494, DateTimeKind.Utc).AddTicks(2461), null, null, "+92", "🇵🇰", false, "Pakistan", null, null });

            migrationBuilder.InsertData(
                table: "EventStatuses",
                columns: new[] { "ID", "CreatedByUserID", "CreatedDateTime", "DeletedByUserID", "DeletedDateTime", "IsDeleted", "Title", "UpdatedByUserID", "UpdatedDateTime" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2026, 9, 17, 13, 48, 17, 493, DateTimeKind.Utc).AddTicks(8382), null, null, false, "Active", null, null },
                    { 2, 1, new DateTime(2026, 9, 17, 13, 48, 17, 493, DateTimeKind.Utc).AddTicks(9395), null, null, false, "Ended", null, null },
                    { 3, 1, new DateTime(2026, 9, 17, 13, 48, 17, 493, DateTimeKind.Utc).AddTicks(9400), null, null, false, "Postponed", null, null }
                });

            migrationBuilder.InsertData(
                table: "PaymentModes",
                columns: new[] { "ID", "CreatedByUserID", "CreatedDateTime", "DeletedByUserID", "DeletedDateTime", "IsDeleted", "Title", "UpdatedByUserID", "UpdatedDateTime" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2026, 9, 17, 13, 48, 17, 494, DateTimeKind.Utc).AddTicks(6512), null, null, false, "Cash", null, null },
                    { 2, 1, new DateTime(2026, 9, 17, 13, 48, 17, 494, DateTimeKind.Utc).AddTicks(6518), null, null, false, "Easypaisa/Jazzcash", null, null },
                    { 3, 1, new DateTime(2026, 9, 17, 13, 48, 17, 494, DateTimeKind.Utc).AddTicks(6519), null, null, false, "Bank Transfer", null, null }
                });

            migrationBuilder.InsertData(
                table: "ReservationStatuses",
                columns: new[] { "ID", "CreatedByUserID", "CreatedDateTime", "DeletedByUserID", "DeletedDateTime", "IsDeleted", "Title", "UpdatedByUserID", "UpdatedDateTime" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2026, 9, 17, 13, 48, 17, 494, DateTimeKind.Utc).AddTicks(8485), null, null, false, "Arrival Pending", null, null },
                    { 2, 1, new DateTime(2026, 9, 17, 13, 48, 17, 494, DateTimeKind.Utc).AddTicks(8491), null, null, false, "Cancelled", null, null },
                    { 3, 1, new DateTime(2026, 9, 17, 13, 48, 17, 494, DateTimeKind.Utc).AddTicks(8492), null, null, false, "Closed", null, null },
                    { 4, 1, new DateTime(2026, 9, 17, 13, 48, 17, 494, DateTimeKind.Utc).AddTicks(8494), null, null, false, "Refunded", null, null },
                    { 5, 1, new DateTime(2026, 9, 17, 13, 48, 17, 494, DateTimeKind.Utc).AddTicks(8496), null, null, false, "Not Attended", null, null }
                });

            migrationBuilder.InsertData(
                table: "Cities",
                columns: new[] { "ID", "CountryID", "CreatedByUserID", "CreatedDateTime", "DeletedByUserID", "DeletedDateTime", "IsDeleted", "Title", "UpdatedByUserID", "UpdatedDateTime" },
                values: new object[,]
                {
                    { 1, 1, 1, new DateTime(2026, 9, 17, 13, 48, 17, 494, DateTimeKind.Utc).AddTicks(4728), null, null, false, "Karachi", null, null },
                    { 2, 1, 1, new DateTime(2026, 9, 17, 13, 48, 17, 494, DateTimeKind.Utc).AddTicks(4734), null, null, false, "Lahore", null, null },
                    { 3, 1, 1, new DateTime(2026, 9, 17, 13, 48, 17, 494, DateTimeKind.Utc).AddTicks(4736), null, null, false, "Islamabad", null, null }
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
                name: "IX_Cities_CountryID",
                table: "Cities",
                column: "CountryID");

            migrationBuilder.CreateIndex(
                name: "IX_Cities_CreatedByUserID",
                table: "Cities",
                column: "CreatedByUserID");

            migrationBuilder.CreateIndex(
                name: "IX_Cities_DeletedByUserID",
                table: "Cities",
                column: "DeletedByUserID");

            migrationBuilder.CreateIndex(
                name: "IX_Cities_UpdatedByUserID",
                table: "Cities",
                column: "UpdatedByUserID");

            migrationBuilder.CreateIndex(
                name: "IX_Columns_CreatedByUserID",
                table: "Columns",
                column: "CreatedByUserID");

            migrationBuilder.CreateIndex(
                name: "IX_Columns_RowID",
                table: "Columns",
                column: "RowID");

            migrationBuilder.CreateIndex(
                name: "IX_Countries_CreatedByUserID",
                table: "Countries",
                column: "CreatedByUserID");

            migrationBuilder.CreateIndex(
                name: "IX_Countries_DeletedByUserID",
                table: "Countries",
                column: "DeletedByUserID");

            migrationBuilder.CreateIndex(
                name: "IX_Countries_UpdatedByUserID",
                table: "Countries",
                column: "UpdatedByUserID");

            migrationBuilder.CreateIndex(
                name: "IX_EventCategories_CategoryID",
                table: "EventCategories",
                column: "CategoryID");

            migrationBuilder.CreateIndex(
                name: "IX_EventCategories_CreatedByUserID",
                table: "EventCategories",
                column: "CreatedByUserID");

            migrationBuilder.CreateIndex(
                name: "IX_EventCategories_EventID",
                table: "EventCategories",
                column: "EventID");

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
                name: "IX_Events_EventStatusID",
                table: "Events",
                column: "EventStatusID");

            migrationBuilder.CreateIndex(
                name: "IX_Events_UpdatedByUserID",
                table: "Events",
                column: "UpdatedByUserID");

            migrationBuilder.CreateIndex(
                name: "IX_EventStatuses_CreatedByUserID",
                table: "EventStatuses",
                column: "CreatedByUserID");

            migrationBuilder.CreateIndex(
                name: "IX_EventStatuses_DeletedByUserID",
                table: "EventStatuses",
                column: "DeletedByUserID");

            migrationBuilder.CreateIndex(
                name: "IX_EventStatuses_UpdatedByUserID",
                table: "EventStatuses",
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
                name: "IX_Notes_CreatedByUserID",
                table: "Notes",
                column: "CreatedByUserID");

            migrationBuilder.CreateIndex(
                name: "IX_Notes_DeletedByUserID",
                table: "Notes",
                column: "DeletedByUserID");

            migrationBuilder.CreateIndex(
                name: "IX_Notes_NoteTypeID",
                table: "Notes",
                column: "NoteTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_Notes_UpdatedByUserID",
                table: "Notes",
                column: "UpdatedByUserID");

            migrationBuilder.CreateIndex(
                name: "IX_NoteTypes_CreatedByUserID",
                table: "NoteTypes",
                column: "CreatedByUserID");

            migrationBuilder.CreateIndex(
                name: "IX_NoteTypes_DeletedByUserID",
                table: "NoteTypes",
                column: "DeletedByUserID");

            migrationBuilder.CreateIndex(
                name: "IX_NoteTypes_UpdatedByUserID",
                table: "NoteTypes",
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
                name: "IX_ReservationImages_ReservationID",
                table: "ReservationImages",
                column: "ReservationID");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_ActionByUserID",
                table: "Reservations",
                column: "ActionByUserID");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_ClosedByUserID",
                table: "Reservations",
                column: "ClosedByUserID");

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
                name: "IX_Reservations_ShowID_IsDeleted_IsApproved",
                table: "Reservations",
                columns: new[] { "ShowID", "IsDeleted", "IsApproved" });

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_TicketNumber",
                table: "Reservations",
                column: "TicketNumber");

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
                name: "IX_ReservationTickets_ReservationID_TicketID",
                table: "ReservationTickets",
                columns: new[] { "ReservationID", "TicketID" });

            migrationBuilder.CreateIndex(
                name: "IX_ReservationTickets_TicketID",
                table: "ReservationTickets",
                column: "TicketID");

            migrationBuilder.CreateIndex(
                name: "IX_Rows_AuditoriumID",
                table: "Rows",
                column: "AuditoriumID");

            migrationBuilder.CreateIndex(
                name: "IX_Rows_CreatedByUserID",
                table: "Rows",
                column: "CreatedByUserID");

            migrationBuilder.CreateIndex(
                name: "IX_Seats_ColumnID",
                table: "Seats",
                column: "ColumnID");

            migrationBuilder.CreateIndex(
                name: "IX_Seats_CreatedByUserID",
                table: "Seats",
                column: "CreatedByUserID");

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
                name: "IX_Shows_EventID_Date_IsDeleted_IsEnded",
                table: "Shows",
                columns: new[] { "EventID", "Date", "IsDeleted", "IsEnded" });

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
                name: "IX_Tickets_ShowID_IsReserved_IsDeleted",
                table: "Tickets",
                columns: new[] { "ShowID", "IsReserved", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_UpdatedByUserID",
                table: "Tickets",
                column: "UpdatedByUserID");

            migrationBuilder.CreateIndex(
                name: "IX_Users_CountryID",
                table: "Users",
                column: "CountryID");

            migrationBuilder.CreateIndex(
                name: "IX_Users_CreatedByUserID",
                table: "Users",
                column: "CreatedByUserID");

            migrationBuilder.CreateIndex(
                name: "IX_Users_DeletedByUserID",
                table: "Users",
                column: "DeletedByUserID");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_Users_RoleID",
                table: "Users",
                column: "RoleID");

            migrationBuilder.CreateIndex(
                name: "IX_Users_UpdatedByUserID",
                table: "Users",
                column: "UpdatedByUserID");

            migrationBuilder.CreateIndex(
                name: "IX_Users_UserToken",
                table: "Users",
                column: "UserToken");

            migrationBuilder.CreateIndex(
                name: "IX_Venues_CityID",
                table: "Venues",
                column: "CityID");

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

            migrationBuilder.AddForeignKey(
                name: "FK_Auditoriums_Users_CreatedByUserID",
                table: "Auditoriums",
                column: "CreatedByUserID",
                principalTable: "Users",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Auditoriums_Users_DeletedByUserID",
                table: "Auditoriums",
                column: "DeletedByUserID",
                principalTable: "Users",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_Auditoriums_Users_UpdatedByUserID",
                table: "Auditoriums",
                column: "UpdatedByUserID",
                principalTable: "Users",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_Auditoriums_Venues_VenueID",
                table: "Auditoriums",
                column: "VenueID",
                principalTable: "Venues",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_Users_CreatedByUserID",
                table: "Categories",
                column: "CreatedByUserID",
                principalTable: "Users",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_Users_DeletedByUserID",
                table: "Categories",
                column: "DeletedByUserID",
                principalTable: "Users",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_Users_UpdatedByUserID",
                table: "Categories",
                column: "UpdatedByUserID",
                principalTable: "Users",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_Cities_Countries_CountryID",
                table: "Cities",
                column: "CountryID",
                principalTable: "Countries",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Cities_Users_CreatedByUserID",
                table: "Cities",
                column: "CreatedByUserID",
                principalTable: "Users",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Cities_Users_DeletedByUserID",
                table: "Cities",
                column: "DeletedByUserID",
                principalTable: "Users",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_Cities_Users_UpdatedByUserID",
                table: "Cities",
                column: "UpdatedByUserID",
                principalTable: "Users",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_Columns_Rows_RowID",
                table: "Columns",
                column: "RowID",
                principalTable: "Rows",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Columns_Users_CreatedByUserID",
                table: "Columns",
                column: "CreatedByUserID",
                principalTable: "Users",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Countries_Users_CreatedByUserID",
                table: "Countries",
                column: "CreatedByUserID",
                principalTable: "Users",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Countries_Users_DeletedByUserID",
                table: "Countries",
                column: "DeletedByUserID",
                principalTable: "Users",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_Countries_Users_UpdatedByUserID",
                table: "Countries",
                column: "UpdatedByUserID",
                principalTable: "Users",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Countries_Users_CreatedByUserID",
                table: "Countries");

            migrationBuilder.DropForeignKey(
                name: "FK_Countries_Users_DeletedByUserID",
                table: "Countries");

            migrationBuilder.DropForeignKey(
                name: "FK_Countries_Users_UpdatedByUserID",
                table: "Countries");

            migrationBuilder.DropTable(
                name: "EventCategories");

            migrationBuilder.DropTable(
                name: "Notes");

            migrationBuilder.DropTable(
                name: "ReservationImages");

            migrationBuilder.DropTable(
                name: "ReservationTickets");

            migrationBuilder.DropTable(
                name: "ShowExpenses");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "NoteTypes");

            migrationBuilder.DropTable(
                name: "Reservations");

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
                name: "EventStatuses");

            migrationBuilder.DropTable(
                name: "Auditoriums");

            migrationBuilder.DropTable(
                name: "Venues");

            migrationBuilder.DropTable(
                name: "Cities");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Countries");

            migrationBuilder.DropTable(
                name: "Roles");
        }
    }
}
