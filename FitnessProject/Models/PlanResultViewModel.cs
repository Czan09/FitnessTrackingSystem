namespace FitnessProject.Models
{
    using FitnessProject.Models.enums;

    public class PlanResultViewModel
    {
        public string? PlanName { get; set; }

        public List<Workout> RecommendedWorkouts { get; set; }
        public List<Diets> RecommendedMeals{get; set;}
        public List<int>? SelectedWorkoutIds { get; set; }
        public string UserId { get; set; } = string.Empty;
    }
}
