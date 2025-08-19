using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FitnessProject.Models
{
    public class WorkoutTags
    {

        [Required]
        public int WorkoutId { get; set; }

        [Required]
        public int TagId { get; set; }

        // Navigation properties
        [ForeignKey("WorkoutId")]
        public Workout? Workout { get; set; }

        [ForeignKey("TagId")]
        public Tags? Tag { get; set; }
    }
}
