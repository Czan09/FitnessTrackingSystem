using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitnessProject.Migrations
{
    /// <inheritdoc />
    public partial class meal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MealPlanTrainers",
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
                    table.PrimaryKey("PK_MealPlanTrainers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MealPlanTrainers_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MealPlanTrainers_TrainerDetails_TrainerId",
                        column: x => x.TrainerId,
                        principalTable: "TrainerDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MealPlanTrainerMeals",
                columns: table => new
                {
                    MealPlanTrainerId = table.Column<int>(type: "int", nullable: false),
                    MealId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MealPlanTrainerMeals", x => new { x.MealPlanTrainerId, x.MealId });
                    table.ForeignKey(
                        name: "FK_MealPlanTrainerMeals_Diets_MealId",
                        column: x => x.MealId,
                        principalTable: "Diets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MealPlanTrainerMeals_MealPlanTrainers_MealPlanTrainerId",
                        column: x => x.MealPlanTrainerId,
                        principalTable: "MealPlanTrainers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MealPlanTrainerMeals_MealId",
                table: "MealPlanTrainerMeals",
                column: "MealId");

            migrationBuilder.CreateIndex(
                name: "IX_MealPlanTrainers_TrainerId",
                table: "MealPlanTrainers",
                column: "TrainerId");

            migrationBuilder.CreateIndex(
                name: "IX_MealPlanTrainers_UserId",
                table: "MealPlanTrainers",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MealPlanTrainerMeals");

            migrationBuilder.DropTable(
                name: "MealPlanTrainers");
        }
    }
}
