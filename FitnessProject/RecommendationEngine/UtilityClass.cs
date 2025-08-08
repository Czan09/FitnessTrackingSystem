namespace FitnessProject.RecommendationEngine
{
    public class UtilityClass
    {
        public static string GetBmiCategory(float bmi)
        {
            if (bmi < 18.5) return "Underweight";
            if (bmi >= 18.5 && bmi < 25) return "Normal";
            if (bmi >= 25 && bmi < 30) return "Overweight";
            return "Obese";
        }
        public static int GetAge(DateOnly dob) => DateTime.Today.Year - dob.Year;


    }
}
