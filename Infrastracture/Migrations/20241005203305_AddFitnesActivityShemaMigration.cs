using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastracture.Migrations
{
    /// <inheritdoc />
    public partial class AddFitnesActivityShemaMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Catalog");

            migrationBuilder.EnsureSchema(
                name: "FitnessActivity");

            migrationBuilder.CreateTable(
                name: "ActivityType",
                schema: "Catalog",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivityType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FitnessActivist",
                schema: "FitnessActivity",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StartTrainingDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    InitialWeight = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CurrentWeight = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Height = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FitnessActivist", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FitnessActivist_User_UserId",
                        column: x => x.UserId,
                        principalSchema: "Auth",
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FitnessActivity",
                schema: "FitnessActivity",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Date = table.Column<DateTime>(type: "Date", nullable: false),
                    Time = table.Column<TimeSpan>(type: "Time", nullable: false),
                    Duration = table.Column<TimeSpan>(type: "Time", nullable: false),
                    ActivityTypeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FitnessActivity", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FitnessActivity_ActivityType_ActivityTypeId",
                        column: x => x.ActivityTypeId,
                        principalSchema: "Catalog",
                        principalTable: "ActivityType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Goal",
                schema: "FitnessActivity",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ActivistId = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ActivityCountGoal = table.Column<int>(type: "int", nullable: true),
                    DurationGoal = table.Column<TimeSpan>(type: "Time", nullable: true),
                    IsAchieved = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Goal", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Goal_FitnessActivist_ActivistId",
                        column: x => x.ActivistId,
                        principalSchema: "FitnessActivity",
                        principalTable: "FitnessActivist",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GoalActivity",
                schema: "FitnessActivity",
                columns: table => new
                {
                    GoalId = table.Column<int>(type: "int", nullable: false),
                    ActivityId = table.Column<int>(type: "int", nullable: false),
                    DesiredCount = table.Column<int>(type: "int", nullable: true),
                    DesiredDuration = table.Column<TimeSpan>(type: "Time", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GoalActivity", x => new { x.GoalId, x.ActivityId });
                    table.ForeignKey(
                        name: "FK_GoalActivity_FitnessActivity_ActivityId",
                        column: x => x.ActivityId,
                        principalSchema: "FitnessActivity",
                        principalTable: "FitnessActivity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GoalActivity_Goal_GoalId",
                        column: x => x.GoalId,
                        principalSchema: "FitnessActivity",
                        principalTable: "Goal",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FitnessActivist_UserId",
                schema: "FitnessActivity",
                table: "FitnessActivist",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_FitnessActivity_ActivityTypeId",
                schema: "FitnessActivity",
                table: "FitnessActivity",
                column: "ActivityTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Goal_ActivistId",
                schema: "FitnessActivity",
                table: "Goal",
                column: "ActivistId");

            migrationBuilder.CreateIndex(
                name: "IX_GoalActivity_ActivityId",
                schema: "FitnessActivity",
                table: "GoalActivity",
                column: "ActivityId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GoalActivity",
                schema: "FitnessActivity");

            migrationBuilder.DropTable(
                name: "FitnessActivity",
                schema: "FitnessActivity");

            migrationBuilder.DropTable(
                name: "Goal",
                schema: "FitnessActivity");

            migrationBuilder.DropTable(
                name: "ActivityType",
                schema: "Catalog");

            migrationBuilder.DropTable(
                name: "FitnessActivist",
                schema: "FitnessActivity");
        }
    }
}
