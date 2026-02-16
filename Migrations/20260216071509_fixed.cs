using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScholaAi.Migrations
{
    /// <inheritdoc />
    public partial class @fixed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RequestBroadcasts_AspNetUsers_TeacherId",
                table: "RequestBroadcasts");

            migrationBuilder.DropForeignKey(
                name: "FK_RequestBroadcasts_SessionRequests_RequestId",
                table: "RequestBroadcasts");

            migrationBuilder.DropForeignKey(
                name: "FK_RequestBroadcasts_Teachers_TeacherApplicationUserId",
                table: "RequestBroadcasts");

            migrationBuilder.DropForeignKey(
                name: "FK_SessionRequests_AspNetUsers_StudentId",
                table: "SessionRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_SessionRequests_AspNetUsers_TeacherId",
                table: "SessionRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_SessionRequests_Students_StudentApplicationUserId",
                table: "SessionRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_SessionRequests_Teachers_TeacherApplicationUserId",
                table: "SessionRequests");

            migrationBuilder.DropIndex(
                name: "IX_SessionRequests_StudentApplicationUserId",
                table: "SessionRequests");

            migrationBuilder.DropIndex(
                name: "IX_SessionRequests_TeacherApplicationUserId",
                table: "SessionRequests");

            migrationBuilder.DropIndex(
                name: "IX_RequestBroadcasts_TeacherApplicationUserId",
                table: "RequestBroadcasts");

            migrationBuilder.DropColumn(
                name: "StudentApplicationUserId",
                table: "SessionRequests");

            migrationBuilder.DropColumn(
                name: "TeacherApplicationUserId",
                table: "SessionRequests");

            migrationBuilder.DropColumn(
                name: "TeacherApplicationUserId",
                table: "RequestBroadcasts");

            migrationBuilder.AddForeignKey(
                name: "FK_RequestBroadcasts_SessionRequests_RequestId",
                table: "RequestBroadcasts",
                column: "RequestId",
                principalTable: "SessionRequests",
                principalColumn: "RequestId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RequestBroadcasts_Teachers_TeacherId",
                table: "RequestBroadcasts",
                column: "TeacherId",
                principalTable: "Teachers",
                principalColumn: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_SessionRequests_Students_StudentId",
                table: "SessionRequests",
                column: "StudentId",
                principalTable: "Students",
                principalColumn: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_SessionRequests_Teachers_TeacherId",
                table: "SessionRequests",
                column: "TeacherId",
                principalTable: "Teachers",
                principalColumn: "ApplicationUserId",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RequestBroadcasts_SessionRequests_RequestId",
                table: "RequestBroadcasts");

            migrationBuilder.DropForeignKey(
                name: "FK_RequestBroadcasts_Teachers_TeacherId",
                table: "RequestBroadcasts");

            migrationBuilder.DropForeignKey(
                name: "FK_SessionRequests_Students_StudentId",
                table: "SessionRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_SessionRequests_Teachers_TeacherId",
                table: "SessionRequests");

            migrationBuilder.AddColumn<string>(
                name: "StudentApplicationUserId",
                table: "SessionRequests",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TeacherApplicationUserId",
                table: "SessionRequests",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TeacherApplicationUserId",
                table: "RequestBroadcasts",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SessionRequests_StudentApplicationUserId",
                table: "SessionRequests",
                column: "StudentApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_SessionRequests_TeacherApplicationUserId",
                table: "SessionRequests",
                column: "TeacherApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestBroadcasts_TeacherApplicationUserId",
                table: "RequestBroadcasts",
                column: "TeacherApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_RequestBroadcasts_AspNetUsers_TeacherId",
                table: "RequestBroadcasts",
                column: "TeacherId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RequestBroadcasts_SessionRequests_RequestId",
                table: "RequestBroadcasts",
                column: "RequestId",
                principalTable: "SessionRequests",
                principalColumn: "RequestId");

            migrationBuilder.AddForeignKey(
                name: "FK_RequestBroadcasts_Teachers_TeacherApplicationUserId",
                table: "RequestBroadcasts",
                column: "TeacherApplicationUserId",
                principalTable: "Teachers",
                principalColumn: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_SessionRequests_AspNetUsers_StudentId",
                table: "SessionRequests",
                column: "StudentId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SessionRequests_AspNetUsers_TeacherId",
                table: "SessionRequests",
                column: "TeacherId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_SessionRequests_Students_StudentApplicationUserId",
                table: "SessionRequests",
                column: "StudentApplicationUserId",
                principalTable: "Students",
                principalColumn: "ApplicationUserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SessionRequests_Teachers_TeacherApplicationUserId",
                table: "SessionRequests",
                column: "TeacherApplicationUserId",
                principalTable: "Teachers",
                principalColumn: "ApplicationUserId");
        }
    }
}
