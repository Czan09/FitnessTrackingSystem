using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitnessProject.Migrations
{
    /// <inheritdoc />
    public partial class test : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Workouts_WorkoutPlanTrainers_WorkoutPlanTrainerId",
                table: "Workouts");

            migrationBuilder.DropIndex(
                name: "IX_Workouts_WorkoutPlanTrainerId",
                table: "Workouts");

            migrationBuilder.DropColumn(
                name: "WorkoutPlanTrainerId",
                table: "Workouts");

            migrationBuilder.CreateTable(
                name: "WorkoutPlanTrainerWorkouts",
                columns: table => new
                {
                    WorkoutPlanTrainerId = table.Column<int>(type: "int", nullable: false),
                    WorkoutId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkoutPlanTrainerWorkouts", x => new { x.WorkoutPlanTrainerId, x.WorkoutId });
                    table.ForeignKey(
                        name: "FK_WorkoutPlanTrainerWorkouts_WorkoutPlanTrainers_WorkoutPlanTrainerId",
                        column: x => x.WorkoutPlanTrainerId,
                        principalTable: "WorkoutPlanTrainers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WorkoutPlanTrainerWorkouts_Workouts_WorkoutId",
                        column: x => x.WorkoutId,
                        principalTable: "Workouts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutPlanTrainerWorkouts_WorkoutId",
                table: "WorkoutPlanTrainerWorkouts",
                column: "WorkoutId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WorkoutPlanTrainerWorkouts");

            migrationBuilder.AddColumn<int>(
                name: "WorkoutPlanTrainerId",
                table: "Workouts",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Workouts_WorkoutPlanTrainerId",
                table: "Workouts",
                column: "WorkoutPlanTrainerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Workouts_WorkoutPlanTrainers_WorkoutPlanTrainerId",
                table: "Workouts",
                column: "WorkoutPlanTrainerId",
                principalTable: "WorkoutPlanTrainers",
                principalColumn: "Id");
        }
    }
}
