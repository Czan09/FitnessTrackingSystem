 using FitnessProject.Models;

namespace FitnessProject.ViewModels
{
    public class WorkoutPlanViewModel
    {
        public string PlanName { get; set; }
        public List<Workout> Workouts { get; set; }
    }
}
