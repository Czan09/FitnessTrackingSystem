using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FitnessProject.Models
{
    public class MealPlanMeals
    {
        [Key]
        public int id { get; set; }
        [Required]
        public int mealPlanId { get; set; }
        [Required]
        public int foodId { get; set; }
        [ForeignKey("foodId")]
        public Diets Meal { get; set; }
    }
}
