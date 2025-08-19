using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitnessProject.Migrations
{
    /// <inheritdoc />
    public partial class ge : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Id",
                table: "WorkoutTags");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "MealTags");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "WorkoutTags",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "MealTags",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
