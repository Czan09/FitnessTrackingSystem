using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FitnessProject.Models
{
    public class MealPlanDiet
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("MealPlan")]
        public int MealPlanId { get; set; }

        public MealPlan MealPlan { get; set; }

        [ForeignKey("Diet")]
        public int DietId { get; set; }

        public Diets Diet { get; set; }

        public string MealTime { get; set; } = string.Empty;
    }
}
