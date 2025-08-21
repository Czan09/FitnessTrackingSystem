using FitnessProject.Models.enums;
using Newtonsoft.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FitnessProject.Models
{
    public class Workout
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public WorkoutType Type { get; set; }

        public string Description { get; set; } = string.Empty;
        [Required]
        [Range (1,60, ErrorMessage ="Duration Should Be between 1 and 60 minutes")]
        public int DurationInMinutes { get; set; }

        [Required]
        [Range(1, 5, ErrorMessage = "Difficulty Should be 1 to 5")]
        public int DifficultyLevel { get; set; }

        public ICollection<WorkoutTags> WorkoutTags { get; set; } = new List<WorkoutTags> ();

    }
}
