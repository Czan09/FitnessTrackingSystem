using FitnessProject.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FitnessProject.Models
{
    public class MealPlan
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string userId { get; set; }
        [ForeignKey("userId")]
        public ApplicationUser User { get; set; }
    }
}
