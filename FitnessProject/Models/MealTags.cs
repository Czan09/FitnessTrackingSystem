using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FitnessProject.Models
{
    public class MealTags
    {

        [Required]
        public int MealId { get; set; }

        [Required]
        public int TagId { get; set; }

        // Navigation properties
        [ForeignKey("MealId")]
        public Diets? Meal { get; set; }

        [ForeignKey("TagId")]
        public Tags? Tag { get; set; }
    }
}
