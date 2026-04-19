using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScholaAi.Migrations
{
    /// <inheritdoc />
    public partial class AddTeacherVerification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "isVerified",
                table: "Teachers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "verificationNotes",
                table: "Teachers",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isVerified",
                table: "Teachers");

            migrationBuilder.DropColumn(
                name: "verificationNotes",
                table: "Teachers");
        }
    }
}
