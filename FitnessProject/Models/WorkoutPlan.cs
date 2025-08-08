using FitnessProject.Models.enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FitnessProject.Models
{
    public class WorkoutPlan
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public int TrainerId { get; set; }

        [ForeignKey("TrainerId")]
        public TrainerDetails? Trainer { get; set; }
        public MemberGoals Goal { get; set; }

        public ICollection<Workout> Workouts { get; set; } = new List<Workout>();
    }
}
