using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MeetingScheduler.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddingWebex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ZoomMeetingPassword",
                table: "Meetings",
                newName: "MeetingPassword");

            migrationBuilder.RenameColumn(
                name: "ZoomMeetingId",
                table: "Meetings",
                newName: "ZoomMeetingId");

            migrationBuilder.AddColumn<bool>(
                name: "IsCeo",
                table: "Meetings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsWebex",
                table: "Meetings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "WebexMeetingNumber",
                table: "Meetings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Keys",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Version = table.Column<int>(type: "int", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Use = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Algorithm = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsX509Certificate = table.Column<bool>(type: "bit", nullable: false),
                    DataProtected = table.Column<bool>(type: "bit", nullable: false),
                    Data = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Keys", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PersistedGrants_ConsumedTime",
                table: "PersistedGrants",
                column: "ConsumedTime");

            migrationBuilder.CreateIndex(
                name: "IX_Keys_Use",
                table: "Keys",
                column: "Use");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Keys");

            migrationBuilder.DropIndex(
                name: "IX_PersistedGrants_ConsumedTime",
                table: "PersistedGrants");

            migrationBuilder.DropColumn(
                name: "IsCeo",
                table: "Meetings");

            migrationBuilder.DropColumn(
                name: "IsWebex",
                table: "Meetings");

            migrationBuilder.DropColumn(
                name: "WebexMeetingNumber",
                table: "Meetings");

            migrationBuilder.RenameColumn(
                name: "MeetingId",
                table: "Meetings",
                newName: "ZoomMeetingId");

            migrationBuilder.RenameColumn(
                name: "MeetingPassword",
                table: "Meetings",
                newName: "ZoomMeetingPassword");
        }
    }
}
