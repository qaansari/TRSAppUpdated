using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TRSAppUpdated.Migrations
{
    /// <inheritdoc />
    public partial class NewReservationStatusAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Cities",
                keyColumn: "ID",
                keyValue: 1,
                column: "CreatedDateTime",
                value: new DateTime(2025, 8, 14, 14, 28, 19, 133, DateTimeKind.Utc).AddTicks(3808));

            migrationBuilder.UpdateData(
                table: "Cities",
                keyColumn: "ID",
                keyValue: 2,
                column: "CreatedDateTime",
                value: new DateTime(2025, 8, 14, 14, 28, 19, 133, DateTimeKind.Utc).AddTicks(3812));

            migrationBuilder.UpdateData(
                table: "Cities",
                keyColumn: "ID",
                keyValue: 3,
                column: "CreatedDateTime",
                value: new DateTime(2025, 8, 14, 14, 28, 19, 133, DateTimeKind.Utc).AddTicks(3815));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "ID",
                keyValue: 1,
                column: "CreatedDateTime",
                value: new DateTime(2025, 8, 14, 14, 28, 19, 133, DateTimeKind.Utc).AddTicks(3618));

            migrationBuilder.UpdateData(
                table: "EventStatuses",
                keyColumn: "ID",
                keyValue: 1,
                column: "CreatedDateTime",
                value: new DateTime(2025, 8, 14, 14, 28, 19, 133, DateTimeKind.Utc).AddTicks(3555));

            migrationBuilder.UpdateData(
                table: "EventStatuses",
                keyColumn: "ID",
                keyValue: 2,
                column: "CreatedDateTime",
                value: new DateTime(2025, 8, 14, 14, 28, 19, 133, DateTimeKind.Utc).AddTicks(3558));

            migrationBuilder.UpdateData(
                table: "EventStatuses",
                keyColumn: "ID",
                keyValue: 3,
                column: "CreatedDateTime",
                value: new DateTime(2025, 8, 14, 14, 28, 19, 133, DateTimeKind.Utc).AddTicks(3560));

            migrationBuilder.UpdateData(
                table: "PaymentModes",
                keyColumn: "ID",
                keyValue: 1,
                column: "CreatedDateTime",
                value: new DateTime(2025, 8, 14, 14, 28, 19, 133, DateTimeKind.Utc).AddTicks(3860));

            migrationBuilder.UpdateData(
                table: "PaymentModes",
                keyColumn: "ID",
                keyValue: 2,
                column: "CreatedDateTime",
                value: new DateTime(2025, 8, 14, 14, 28, 19, 133, DateTimeKind.Utc).AddTicks(3861));

            migrationBuilder.UpdateData(
                table: "PaymentModes",
                keyColumn: "ID",
                keyValue: 3,
                column: "CreatedDateTime",
                value: new DateTime(2025, 8, 14, 14, 28, 19, 133, DateTimeKind.Utc).AddTicks(3864));

            migrationBuilder.UpdateData(
                table: "ReservationStatuses",
                keyColumn: "ID",
                keyValue: 1,
                column: "CreatedDateTime",
                value: new DateTime(2025, 8, 14, 14, 28, 19, 133, DateTimeKind.Utc).AddTicks(3933));

            migrationBuilder.UpdateData(
                table: "ReservationStatuses",
                keyColumn: "ID",
                keyValue: 2,
                column: "CreatedDateTime",
                value: new DateTime(2025, 8, 14, 14, 28, 19, 133, DateTimeKind.Utc).AddTicks(3934));

            migrationBuilder.UpdateData(
                table: "ReservationStatuses",
                keyColumn: "ID",
                keyValue: 3,
                column: "CreatedDateTime",
                value: new DateTime(2025, 8, 14, 14, 28, 19, 133, DateTimeKind.Utc).AddTicks(3936));

            migrationBuilder.UpdateData(
                table: "ReservationStatuses",
                keyColumn: "ID",
                keyValue: 4,
                column: "CreatedDateTime",
                value: new DateTime(2025, 8, 14, 14, 28, 19, 133, DateTimeKind.Utc).AddTicks(3938));

            migrationBuilder.InsertData(
                table: "ReservationStatuses",
                columns: new[] { "ID", "CreatedByUserID", "CreatedDateTime", "DeletedByUserID", "DeletedDateTime", "IsDeleted", "Title", "UpdatedByUserID", "UpdatedDateTime" },
                values: new object[] { 5, 1, new DateTime(2025, 8, 14, 14, 28, 19, 133, DateTimeKind.Utc).AddTicks(3942), null, null, false, "Not Attended", null, null });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "ID",
                keyValue: 1,
                column: "CreatedDateTime",
                value: new DateTime(2025, 8, 14, 14, 28, 19, 133, DateTimeKind.Utc).AddTicks(3017));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ReservationStatuses",
                keyColumn: "ID",
                keyValue: 5);

            migrationBuilder.UpdateData(
                table: "Cities",
                keyColumn: "ID",
                keyValue: 1,
                column: "CreatedDateTime",
                value: new DateTime(2025, 7, 28, 18, 7, 6, 617, DateTimeKind.Utc).AddTicks(867));

            migrationBuilder.UpdateData(
                table: "Cities",
                keyColumn: "ID",
                keyValue: 2,
                column: "CreatedDateTime",
                value: new DateTime(2025, 7, 28, 18, 7, 6, 617, DateTimeKind.Utc).AddTicks(869));

            migrationBuilder.UpdateData(
                table: "Cities",
                keyColumn: "ID",
                keyValue: 3,
                column: "CreatedDateTime",
                value: new DateTime(2025, 7, 28, 18, 7, 6, 617, DateTimeKind.Utc).AddTicks(871));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "ID",
                keyValue: 1,
                column: "CreatedDateTime",
                value: new DateTime(2025, 7, 28, 18, 7, 6, 617, DateTimeKind.Utc).AddTicks(831));

            migrationBuilder.UpdateData(
                table: "EventStatuses",
                keyColumn: "ID",
                keyValue: 1,
                column: "CreatedDateTime",
                value: new DateTime(2025, 7, 28, 18, 7, 6, 617, DateTimeKind.Utc).AddTicks(776));

            migrationBuilder.UpdateData(
                table: "EventStatuses",
                keyColumn: "ID",
                keyValue: 2,
                column: "CreatedDateTime",
                value: new DateTime(2025, 7, 28, 18, 7, 6, 617, DateTimeKind.Utc).AddTicks(779));

            migrationBuilder.UpdateData(
                table: "EventStatuses",
                keyColumn: "ID",
                keyValue: 3,
                column: "CreatedDateTime",
                value: new DateTime(2025, 7, 28, 18, 7, 6, 617, DateTimeKind.Utc).AddTicks(780));

            migrationBuilder.UpdateData(
                table: "PaymentModes",
                keyColumn: "ID",
                keyValue: 1,
                column: "CreatedDateTime",
                value: new DateTime(2025, 7, 28, 18, 7, 6, 617, DateTimeKind.Utc).AddTicks(903));

            migrationBuilder.UpdateData(
                table: "PaymentModes",
                keyColumn: "ID",
                keyValue: 2,
                column: "CreatedDateTime",
                value: new DateTime(2025, 7, 28, 18, 7, 6, 617, DateTimeKind.Utc).AddTicks(905));

            migrationBuilder.UpdateData(
                table: "PaymentModes",
                keyColumn: "ID",
                keyValue: 3,
                column: "CreatedDateTime",
                value: new DateTime(2025, 7, 28, 18, 7, 6, 617, DateTimeKind.Utc).AddTicks(906));

            migrationBuilder.UpdateData(
                table: "ReservationStatuses",
                keyColumn: "ID",
                keyValue: 1,
                column: "CreatedDateTime",
                value: new DateTime(2025, 7, 28, 18, 7, 6, 617, DateTimeKind.Utc).AddTicks(943));

            migrationBuilder.UpdateData(
                table: "ReservationStatuses",
                keyColumn: "ID",
                keyValue: 2,
                column: "CreatedDateTime",
                value: new DateTime(2025, 7, 28, 18, 7, 6, 617, DateTimeKind.Utc).AddTicks(944));

            migrationBuilder.UpdateData(
                table: "ReservationStatuses",
                keyColumn: "ID",
                keyValue: 3,
                column: "CreatedDateTime",
                value: new DateTime(2025, 7, 28, 18, 7, 6, 617, DateTimeKind.Utc).AddTicks(945));

            migrationBuilder.UpdateData(
                table: "ReservationStatuses",
                keyColumn: "ID",
                keyValue: 4,
                column: "CreatedDateTime",
                value: new DateTime(2025, 7, 28, 18, 7, 6, 617, DateTimeKind.Utc).AddTicks(947));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "ID",
                keyValue: 1,
                column: "CreatedDateTime",
                value: new DateTime(2025, 7, 28, 18, 7, 6, 617, DateTimeKind.Utc).AddTicks(455));
        }
    }
}
