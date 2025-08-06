using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FitnessProject.Models.enums;

namespace FitnessProject.Models
{
    public class Diets
    {
        public int Id { get; set; }

        [Required]
        public string MealName { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public int Calories { get; set; }

        public DietType Type { get; set; }


    }
}
