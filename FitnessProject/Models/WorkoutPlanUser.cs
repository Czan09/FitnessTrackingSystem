using FitnessProject.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FitnessProject.Models
{
    public class WorkoutPlanUser
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string UserId { get; set; }= string.Empty;
        [Required]
        public int WorkoutId { get; set; }
        [ForeignKey("WorkoutId")]
        public Workout? Workout { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser? User { get; set; }
    }
}
