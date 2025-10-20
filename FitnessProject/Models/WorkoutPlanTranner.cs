using FitnessProject.Data;
using FitnessProject.Models;
using NuGet.DependencyResolver;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FitnessProject.Models
{
    public class WorkoutPlanTrainer
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string PlanName { get; set; } = string.Empty;

        [Required]
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser? User { get; set; }

        public int TrainerId { get; set; }
        public TrainerDetails? Trainer { get; set; }

        public List<WorkoutPlanTrainerWorkout> WorkoutPlanTrainerWorkouts { get; set; } = new();

        [NotMapped] // for binding checkboxes
        public List<int> WorkoutIds { get; set; } = new();
    }

}