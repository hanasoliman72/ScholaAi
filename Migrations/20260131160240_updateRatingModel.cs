using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScholaAi.Migrations
{
    /// <inheritdoc />
    public partial class updateRatingModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "comment",
                table: "ratings",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "createdAt",
                table: "ratings",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "studentId",
                table: "ratings",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ratings_studentId",
                table: "ratings",
                column: "studentId");

            migrationBuilder.AddForeignKey(
                name: "FK_ratings_users_studentId",
                table: "ratings",
                column: "studentId",
                principalTable: "users",
                principalColumn: "userId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ratings_users_studentId",
                table: "ratings");

            migrationBuilder.DropIndex(
                name: "IX_ratings_studentId",
                table: "ratings");

            migrationBuilder.DropColumn(
                name: "comment",
                table: "ratings");

            migrationBuilder.DropColumn(
                name: "createdAt",
                table: "ratings");

            migrationBuilder.DropColumn(
                name: "studentId",
                table: "ratings");
        }
    }
}
