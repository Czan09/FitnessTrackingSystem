namespace FitnessProject.Models
{
    public class MealPlanTrainerMeal
    {
        public int MealPlanTrainerId { get; set; }
        public MealPlanTrainer? MealPlanTrainer { get; set; } = null;

        public int MealId { get; set; }
        public Diets? Meal { get; set; } = null;// Assuming you have a Meals table
    }
}
