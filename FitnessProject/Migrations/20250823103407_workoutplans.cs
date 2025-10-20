using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitnessProject.Migrations
{
    /// <inheritdoc />
    public partial class workoutplans : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "WorkoutPlanTrainerId",
                table: "Workouts",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "TrainerDetails",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "WorkoutPlanTrainers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlanName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TrainerId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkoutPlanTrainers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkoutPlanTrainers_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_WorkoutPlanTrainers_TrainerDetails_TrainerId",
                        column: x => x.TrainerId,
                        principalTable: "TrainerDetails",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Workouts_WorkoutPlanTrainerId",
                table: "Workouts",
                column: "WorkoutPlanTrainerId");

            migrationBuilder.CreateIndex(
                name: "IX_TrainerDetails_ApplicationUserId",
                table: "TrainerDetails",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutPlanTrainers_TrainerId",
                table: "WorkoutPlanTrainers",
                column: "TrainerId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutPlanTrainers_UserId",
                table: "WorkoutPlanTrainers",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_TrainerDetails_AspNetUsers_ApplicationUserId",
                table: "TrainerDetails",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Workouts_WorkoutPlanTrainers_WorkoutPlanTrainerId",
                table: "Workouts",
                column: "WorkoutPlanTrainerId",
                principalTable: "WorkoutPlanTrainers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TrainerDetails_AspNetUsers_ApplicationUserId",
                table: "TrainerDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_Workouts_WorkoutPlanTrainers_WorkoutPlanTrainerId",
                table: "Workouts");

            migrationBuilder.DropTable(
                name: "WorkoutPlanTrainers");

            migrationBuilder.DropIndex(
                name: "IX_Workouts_WorkoutPlanTrainerId",
                table: "Workouts");

            migrationBuilder.DropIndex(
                name: "IX_TrainerDetails_ApplicationUserId",
                table: "TrainerDetails");

            migrationBuilder.DropColumn(
                name: "WorkoutPlanTrainerId",
                table: "Workouts");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "TrainerDetails");
        }
    }
}
