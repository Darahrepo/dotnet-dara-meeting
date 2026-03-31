using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MeetingScheduler.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameMeetingId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
           name: "ZoomMeetingId",
           table: "Meetings",
           newName: "MeetingId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MeetingId",
                table: "Meetings",
                newName: "ZoomMeetingId");
        }
    }
}
