using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitnessProject.Migrations
{
    /// <inheritdoc />
    public partial class changePlan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Goal",
                table: "WorkoutPlans",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Goal",
                table: "WorkoutPlans");
        }
    }
}
