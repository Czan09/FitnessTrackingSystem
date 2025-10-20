using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FitnessProject.Data;

namespace FitnessProject.Models
{
    public class MealPlanTrainer
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string PlanName { get; set; } = string.Empty;

        [Required]
        public string UserId { get; set; }
        public ApplicationUser? User { get; set; }

        [Required]
        public int TrainerId { get; set; }
        public TrainerDetails? Trainer { get; set; }

        // Many-to-many join table
        public List<MealPlanTrainerMeal> MealPlanTrainerMeals { get; set; } = new();

        // Convenience property to access Meals directly
        [NotMapped]
        public IEnumerable<Diets> Meals
        {
            get
            {
                return MealPlanTrainerMeals?.Select(m => m.Meal) ?? Enumerable.Empty<Diets>();
            }
        }

    }
}
