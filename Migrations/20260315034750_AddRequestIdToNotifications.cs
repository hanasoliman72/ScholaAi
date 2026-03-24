using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScholaAi.Migrations
{
    /// <inheritdoc />
    public partial class AddRequestIdToNotifications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RequestId",
                table: "Notifications",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_RequestId",
                table: "Notifications",
                column: "RequestId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_SessionRequests_RequestId",
                table: "Notifications",
                column: "RequestId",
                principalTable: "SessionRequests",
                principalColumn: "RequestId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_SessionRequests_RequestId",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_RequestId",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "RequestId",
                table: "Notifications");
        }
    }
}
