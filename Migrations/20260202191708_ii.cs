using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScholaAi.Migrations
{
    /// <inheritdoc />
    public partial class ii : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_sessionRequests_subjects_subjectId",
                table: "sessionRequests");

            migrationBuilder.DropTable(
                name: "teacherSubjects");

            migrationBuilder.DropIndex(
                name: "IX_sessionRequests_subjectId",
                table: "sessionRequests");

            migrationBuilder.RenameColumn(
                name: "sessionId",
                table: "sessionRequests",
                newName: "requestId");

            migrationBuilder.AddColumn<int>(
                name: "subjectId",
                table: "teachers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "description",
                table: "subjects",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_teachers_subjectId",
                table: "teachers",
                column: "subjectId");

            migrationBuilder.CreateIndex(
                name: "IX_sessionRequests_subjectId",
                table: "sessionRequests",
                column: "subjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_sessionRequests_subjects_subjectId",
                table: "sessionRequests",
                column: "subjectId",
                principalTable: "subjects",
                principalColumn: "subjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_teachers_subjects_subjectId",
                table: "teachers",
                column: "subjectId",
                principalTable: "subjects",
                principalColumn: "subjectId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_sessionRequests_subjects_subjectId",
                table: "sessionRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_teachers_subjects_subjectId",
                table: "teachers");

            migrationBuilder.DropIndex(
                name: "IX_teachers_subjectId",
                table: "teachers");

            migrationBuilder.DropIndex(
                name: "IX_sessionRequests_subjectId",
                table: "sessionRequests");

            migrationBuilder.DropColumn(
                name: "subjectId",
                table: "teachers");

            migrationBuilder.RenameColumn(
                name: "requestId",
                table: "sessionRequests",
                newName: "sessionId");

            migrationBuilder.AlterColumn<string>(
                name: "description",
                table: "subjects",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "teacherSubjects",
                columns: table => new
                {
                    subjectId = table.Column<int>(type: "int", nullable: false),
                    teacherId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_teacherSubjects", x => new { x.subjectId, x.teacherId });
                    table.ForeignKey(
                        name: "FK_teacherSubjects_subjects_subjectId",
                        column: x => x.subjectId,
                        principalTable: "subjects",
                        principalColumn: "subjectId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_teacherSubjects_teachers_teacherId",
                        column: x => x.teacherId,
                        principalTable: "teachers",
                        principalColumn: "userId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_sessionRequests_subjectId",
                table: "sessionRequests",
                column: "subjectId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_teacherSubjects_teacherId",
                table: "teacherSubjects",
                column: "teacherId");

            migrationBuilder.AddForeignKey(
                name: "FK_sessionRequests_subjects_subjectId",
                table: "sessionRequests",
                column: "subjectId",
                principalTable: "subjects",
                principalColumn: "subjectId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
