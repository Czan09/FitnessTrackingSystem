using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitnessProject.Migrations
{
    /// <inheritdoc />
    public partial class removeplan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Workouts_WorkoutPlans_WorkoutPlanId",
                table: "Workouts");

            migrationBuilder.DropTable(
                name: "WorkoutPlans");

            migrationBuilder.DropIndex(
                name: "IX_Workouts_WorkoutPlanId",
                table: "Workouts");

            migrationBuilder.DropColumn(
                name: "WorkoutPlanId",
                table: "Workouts");

            migrationBuilder.CreateIndex(
                name: "IX_MealPlanMeals_foodId",
                table: "MealPlanMeals",
                column: "foodId");

            migrationBuilder.AddForeignKey(
                name: "FK_MealPlanMeals_Diets_foodId",
                table: "MealPlanMeals",
                column: "foodId",
                principalTable: "Diets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MealPlanMeals_Diets_foodId",
                table: "MealPlanMeals");

            migrationBuilder.DropIndex(
                name: "IX_MealPlanMeals_foodId",
                table: "MealPlanMeals");

            migrationBuilder.AddColumn<int>(
                name: "WorkoutPlanId",
                table: "Workouts",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "WorkoutPlans",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TrainerId = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Goal = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserFitnessDetailsId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkoutPlans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkoutPlans_TrainerDetails_TrainerId",
                        column: x => x.TrainerId,
                        principalTable: "TrainerDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WorkoutPlans_UserFitnessDetails_UserFitnessDetailsId",
                        column: x => x.UserFitnessDetailsId,
                        principalTable: "UserFitnessDetails",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Workouts_WorkoutPlanId",
                table: "Workouts",
                column: "WorkoutPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutPlans_TrainerId",
                table: "WorkoutPlans",
                column: "TrainerId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutPlans_UserFitnessDetailsId",
                table: "WorkoutPlans",
                column: "UserFitnessDetailsId");

            migrationBuilder.AddForeignKey(
                name: "FK_Workouts_WorkoutPlans_WorkoutPlanId",
                table: "Workouts",
                column: "WorkoutPlanId",
                principalTable: "WorkoutPlans",
                principalColumn: "Id");
        }
    }
}
