using Microsoft.ML.Data;

namespace FitnessProject.RecommendationEngine
{
    public class Models
    {
        public class UserFitnessInput
        {
            public float Age { get; set; }
            public float BMI { get; set; }
            public string Gender { get; set; } 
            public string ExperienceLevel { get; set; } 
            public string Goal { get; set; } 
            public string DietType { get; set; }  
        }

        public class PlanOutput
        {
            [ColumnName("PredictedLabel")]
            public string RecommendedWorkoutPlanId { get; set; }

            public float[] Score { get; set; } 
        }

        public class MealPlanOutput
        {
            [ColumnName("PredictedLabel")]
            public string RecommendedMealPlanId { get; set; }

            public float[] Score { get; set; }
        }

    }
}
