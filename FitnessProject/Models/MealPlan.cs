using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace FitnessProject.Models
{
    public class MealPlan
    {
        [Key]
        public int id { get; set; }
        [Required]
        public string PlanName { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string Day { get; set; } = string.Empty;

        public int TotalCalories { get; set; }

        public ICollection<MealPlanDiet> Meals { get; set; } = new List<MealPlanDiet>();

    }
}
