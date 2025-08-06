using System.ComponentModel.DataAnnotations;

namespace FitnessProject.Models
{
    public class TrainerDetails
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        public string Specialty { get; set; } = string.Empty;

        public int YearsOfExperience { get; set; }

        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        public string Contact { get; set; } = string.Empty;

        // Navigation
        public ICollection<WorkoutPlan> WorkoutPlans { get; set; } = new List<WorkoutPlan>();
    }
}
