using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScholaAi.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "subjects",
                columns: table => new
                {
                    subjectId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_subjects", x => x.subjectId);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    userId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    userName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    passwordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    userType = table.Column<int>(type: "int", nullable: false),
                    profilePhotoURL = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    firstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    lastName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.userId);
                });

            migrationBuilder.CreateTable(
                name: "chatMessages",
                columns: table => new
                {
                    messageId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    senderId = table.Column<int>(type: "int", nullable: false),
                    receiverId = table.Column<int>(type: "int", nullable: false),
                    messageText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    attachmentURL = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    isRead = table.Column<bool>(type: "bit", nullable: false),
                    sentAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_chatMessages", x => x.messageId);
                    table.ForeignKey(
                        name: "FK_chatMessages_users_receiverId",
                        column: x => x.receiverId,
                        principalTable: "users",
                        principalColumn: "userId");
                    table.ForeignKey(
                        name: "FK_chatMessages_users_senderId",
                        column: x => x.senderId,
                        principalTable: "users",
                        principalColumn: "userId");
                });

            migrationBuilder.CreateTable(
                name: "students",
                columns: table => new
                {
                    userId = table.Column<int>(type: "int", nullable: false),
                    grade = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_students", x => x.userId);
                    table.ForeignKey(
                        name: "FK_students_users_userId",
                        column: x => x.userId,
                        principalTable: "users",
                        principalColumn: "userId");
                });

            migrationBuilder.CreateTable(
                name: "teachers",
                columns: table => new
                {
                    userId = table.Column<int>(type: "int", nullable: false),
                    college = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    certificate = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    totalHoursTaught = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    totalRates = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_teachers", x => x.userId);
                    table.ForeignKey(
                        name: "FK_teachers_users_userId",
                        column: x => x.userId,
                        principalTable: "users",
                        principalColumn: "userId");
                });

            migrationBuilder.CreateTable(
                name: "wallets",
                columns: table => new
                {
                    walletId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    userId = table.Column<int>(type: "int", nullable: false),
                    balance = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    updatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_wallets", x => x.walletId);
                    table.ForeignKey(
                        name: "FK_wallets_users_userId",
                        column: x => x.userId,
                        principalTable: "users",
                        principalColumn: "userId");
                });

            migrationBuilder.CreateTable(
                name: "sessionRequests",
                columns: table => new
                {
                    sessionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    teacherId = table.Column<int>(type: "int", nullable: true),
                    studentId = table.Column<int>(type: "int", nullable: false),
                    subjectId = table.Column<int>(type: "int", nullable: false),
                    preferredDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    status = table.Column<int>(type: "int", nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    finalScheduledAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    createdAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sessionRequests", x => x.sessionId);
                    table.ForeignKey(
                        name: "FK_sessionRequests_students_studentId",
                        column: x => x.studentId,
                        principalTable: "students",
                        principalColumn: "userId");
                    table.ForeignKey(
                        name: "FK_sessionRequests_subjects_subjectId",
                        column: x => x.subjectId,
                        principalTable: "subjects",
                        principalColumn: "subjectId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_sessionRequests_teachers_teacherId",
                        column: x => x.teacherId,
                        principalTable: "teachers",
                        principalColumn: "userId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "teacherSubjects",
                columns: table => new
                {
                    teacherId = table.Column<int>(type: "int", nullable: false),
                    subjectId = table.Column<int>(type: "int", nullable: false)
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

            migrationBuilder.CreateTable(
                name: "adminLogs",
                columns: table => new
                {
                    logId = table.Column<int>(type: "int", nullable: false),
                    adminId = table.Column<int>(type: "int", nullable: false),
                    targetType = table.Column<int>(type: "int", nullable: true),
                    targetUserId = table.Column<int>(type: "int", nullable: true),
                    targetRequestId = table.Column<int>(type: "int", nullable: true),
                    createdAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    details = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_adminLogs", x => x.logId);
                    table.ForeignKey(
                        name: "FK_adminLogs_sessionRequests_targetRequestId",
                        column: x => x.targetRequestId,
                        principalTable: "sessionRequests",
                        principalColumn: "sessionId");
                    table.ForeignKey(
                        name: "FK_adminLogs_users_logId",
                        column: x => x.logId,
                        principalTable: "users",
                        principalColumn: "userId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_adminLogs_users_targetUserId",
                        column: x => x.targetUserId,
                        principalTable: "users",
                        principalColumn: "userId");
                });

            migrationBuilder.CreateTable(
                name: "requestBroadcasts",
                columns: table => new
                {
                    broadcastId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    requestId = table.Column<int>(type: "int", nullable: false),
                    teacherId = table.Column<int>(type: "int", nullable: false),
                    sentAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    isDelivered = table.Column<bool>(type: "bit", nullable: false),
                    isAccepted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_requestBroadcasts", x => x.broadcastId);
                    table.ForeignKey(
                        name: "FK_requestBroadcasts_sessionRequests_requestId",
                        column: x => x.requestId,
                        principalTable: "sessionRequests",
                        principalColumn: "sessionId");
                    table.ForeignKey(
                        name: "FK_requestBroadcasts_teachers_teacherId",
                        column: x => x.teacherId,
                        principalTable: "teachers",
                        principalColumn: "userId");
                });

            migrationBuilder.CreateTable(
                name: "sessions",
                columns: table => new
                {
                    sessionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    requestId = table.Column<int>(type: "int", nullable: false),
                    teacherId = table.Column<int>(type: "int", nullable: false),
                    studentId = table.Column<int>(type: "int", nullable: false),
                    recordedSession = table.Column<long>(type: "bigint", nullable: false),
                    summary = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sessions", x => x.sessionId);
                    table.ForeignKey(
                        name: "FK_sessions_sessionRequests_requestId",
                        column: x => x.requestId,
                        principalTable: "sessionRequests",
                        principalColumn: "sessionId");
                    table.ForeignKey(
                        name: "FK_sessions_students_studentId",
                        column: x => x.studentId,
                        principalTable: "students",
                        principalColumn: "userId");
                    table.ForeignKey(
                        name: "FK_sessions_teachers_teacherId",
                        column: x => x.teacherId,
                        principalTable: "teachers",
                        principalColumn: "userId");
                });

            migrationBuilder.CreateTable(
                name: "notifications",
                columns: table => new
                {
                    notificationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    receiverId = table.Column<int>(type: "int", nullable: false),
                    senderId = table.Column<int>(type: "int", nullable: false),
                    sessionId = table.Column<int>(type: "int", nullable: false),
                    message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    createdAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    isRead = table.Column<bool>(type: "bit", nullable: false),
                    type = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_notifications", x => x.notificationId);
                    table.ForeignKey(
                        name: "FK_notifications_sessions_sessionId",
                        column: x => x.sessionId,
                        principalTable: "sessions",
                        principalColumn: "sessionId");
                    table.ForeignKey(
                        name: "FK_notifications_users_receiverId",
                        column: x => x.receiverId,
                        principalTable: "users",
                        principalColumn: "userId");
                    table.ForeignKey(
                        name: "FK_notifications_users_senderId",
                        column: x => x.senderId,
                        principalTable: "users",
                        principalColumn: "userId");
                });

            migrationBuilder.CreateTable(
                name: "ratings",
                columns: table => new
                {
                    ratingId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    sessionId = table.Column<int>(type: "int", nullable: false),
                    teacherId = table.Column<int>(type: "int", nullable: false),
                    ratingValue = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ratings", x => x.ratingId);
                    table.ForeignKey(
                        name: "FK_ratings_sessions_sessionId",
                        column: x => x.sessionId,
                        principalTable: "sessions",
                        principalColumn: "sessionId");
                    table.ForeignKey(
                        name: "FK_ratings_teachers_teacherId",
                        column: x => x.teacherId,
                        principalTable: "teachers",
                        principalColumn: "userId");
                });

            migrationBuilder.CreateTable(
                name: "transactions",
                columns: table => new
                {
                    transactionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    fromWalletId = table.Column<int>(type: "int", nullable: false),
                    toWalletId = table.Column<int>(type: "int", nullable: false),
                    sessionId = table.Column<int>(type: "int", nullable: false),
                    amount = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    platformFee = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    createdAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_transactions", x => x.transactionId);
                    table.ForeignKey(
                        name: "FK_transactions_sessions_sessionId",
                        column: x => x.sessionId,
                        principalTable: "sessions",
                        principalColumn: "sessionId");
                    table.ForeignKey(
                        name: "FK_transactions_wallets_fromWalletId",
                        column: x => x.fromWalletId,
                        principalTable: "wallets",
                        principalColumn: "walletId");
                    table.ForeignKey(
                        name: "FK_transactions_wallets_toWalletId",
                        column: x => x.toWalletId,
                        principalTable: "wallets",
                        principalColumn: "walletId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_adminLogs_targetRequestId",
                table: "adminLogs",
                column: "targetRequestId",
                unique: true,
                filter: "[targetRequestId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_adminLogs_targetUserId",
                table: "adminLogs",
                column: "targetUserId",
                unique: true,
                filter: "[targetUserId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_chatMessages_receiverId",
                table: "chatMessages",
                column: "receiverId");

            migrationBuilder.CreateIndex(
                name: "IX_chatMessages_senderId",
                table: "chatMessages",
                column: "senderId");

            migrationBuilder.CreateIndex(
                name: "IX_notifications_receiverId",
                table: "notifications",
                column: "receiverId");

            migrationBuilder.CreateIndex(
                name: "IX_notifications_senderId",
                table: "notifications",
                column: "senderId");

            migrationBuilder.CreateIndex(
                name: "IX_notifications_sessionId",
                table: "notifications",
                column: "sessionId");

            migrationBuilder.CreateIndex(
                name: "IX_ratings_sessionId",
                table: "ratings",
                column: "sessionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ratings_teacherId",
                table: "ratings",
                column: "teacherId");

            migrationBuilder.CreateIndex(
                name: "IX_requestBroadcasts_requestId",
                table: "requestBroadcasts",
                column: "requestId");

            migrationBuilder.CreateIndex(
                name: "IX_requestBroadcasts_teacherId",
                table: "requestBroadcasts",
                column: "teacherId");

            migrationBuilder.CreateIndex(
                name: "IX_sessionRequests_studentId",
                table: "sessionRequests",
                column: "studentId");

            migrationBuilder.CreateIndex(
                name: "IX_sessionRequests_subjectId",
                table: "sessionRequests",
                column: "subjectId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_sessionRequests_teacherId",
                table: "sessionRequests",
                column: "teacherId");

            migrationBuilder.CreateIndex(
                name: "IX_sessions_requestId",
                table: "sessions",
                column: "requestId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_sessions_studentId",
                table: "sessions",
                column: "studentId");

            migrationBuilder.CreateIndex(
                name: "IX_sessions_teacherId",
                table: "sessions",
                column: "teacherId");

            migrationBuilder.CreateIndex(
                name: "IX_teacherSubjects_teacherId",
                table: "teacherSubjects",
                column: "teacherId");

            migrationBuilder.CreateIndex(
                name: "IX_transactions_fromWalletId",
                table: "transactions",
                column: "fromWalletId");

            migrationBuilder.CreateIndex(
                name: "IX_transactions_sessionId",
                table: "transactions",
                column: "sessionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_transactions_toWalletId",
                table: "transactions",
                column: "toWalletId");

            migrationBuilder.CreateIndex(
                name: "IX_users_email",
                table: "users",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_users_userName",
                table: "users",
                column: "userName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_wallets_userId",
                table: "wallets",
                column: "userId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "adminLogs");

            migrationBuilder.DropTable(
                name: "chatMessages");

            migrationBuilder.DropTable(
                name: "notifications");

            migrationBuilder.DropTable(
                name: "ratings");

            migrationBuilder.DropTable(
                name: "requestBroadcasts");

            migrationBuilder.DropTable(
                name: "teacherSubjects");

            migrationBuilder.DropTable(
                name: "transactions");

            migrationBuilder.DropTable(
                name: "sessions");

            migrationBuilder.DropTable(
                name: "wallets");

            migrationBuilder.DropTable(
                name: "sessionRequests");

            migrationBuilder.DropTable(
                name: "students");

            migrationBuilder.DropTable(
                name: "subjects");

            migrationBuilder.DropTable(
                name: "teachers");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
