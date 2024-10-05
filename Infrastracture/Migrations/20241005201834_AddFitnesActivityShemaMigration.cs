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
                name: "ActivityTypes",
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
                    table.PrimaryKey("PK_ActivityTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FitnessActivists",
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
                    table.PrimaryKey("PK_FitnessActivists", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FitnessActivists_User_UserId",
                        column: x => x.UserId,
                        principalSchema: "Auth",
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FitnessActivities",
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
                    table.PrimaryKey("PK_FitnessActivities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FitnessActivities_ActivityTypes_ActivityTypeId",
                        column: x => x.ActivityTypeId,
                        principalSchema: "Catalog",
                        principalTable: "ActivityTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Goals",
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
                    table.PrimaryKey("PK_Goals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Goals_FitnessActivists_ActivistId",
                        column: x => x.ActivistId,
                        principalSchema: "FitnessActivity",
                        principalTable: "FitnessActivists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GoalActivities",
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
                    table.PrimaryKey("PK_GoalActivities", x => new { x.GoalId, x.ActivityId });
                    table.ForeignKey(
                        name: "FK_GoalActivities_FitnessActivities_ActivityId",
                        column: x => x.ActivityId,
                        principalSchema: "FitnessActivity",
                        principalTable: "FitnessActivities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GoalActivities_Goals_GoalId",
                        column: x => x.GoalId,
                        principalSchema: "FitnessActivity",
                        principalTable: "Goals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FitnessActivists_UserId",
                schema: "FitnessActivity",
                table: "FitnessActivists",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_FitnessActivities_ActivityTypeId",
                schema: "FitnessActivity",
                table: "FitnessActivities",
                column: "ActivityTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_GoalActivities_ActivityId",
                schema: "FitnessActivity",
                table: "GoalActivities",
                column: "ActivityId");

            migrationBuilder.CreateIndex(
                name: "IX_Goals_ActivistId",
                schema: "FitnessActivity",
                table: "Goals",
                column: "ActivistId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GoalActivities",
                schema: "FitnessActivity");

            migrationBuilder.DropTable(
                name: "FitnessActivities",
                schema: "FitnessActivity");

            migrationBuilder.DropTable(
                name: "Goals",
                schema: "FitnessActivity");

            migrationBuilder.DropTable(
                name: "ActivityTypes",
                schema: "Catalog");

            migrationBuilder.DropTable(
                name: "FitnessActivists",
                schema: "FitnessActivity");
        }
    }
}
