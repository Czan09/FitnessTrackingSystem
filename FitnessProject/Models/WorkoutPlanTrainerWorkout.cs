namespace FitnessProject.Models
{
    public class WorkoutPlanTrainerWorkout
    {
        public int WorkoutPlanTrainerId { get; set; }
        public WorkoutPlanTrainer WorkoutPlanTrainer { get; set; } = null!;

        public int WorkoutId { get; set; }
        public Workout Workout { get; set; } = null!;
    }

}
